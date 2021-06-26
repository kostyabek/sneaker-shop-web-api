using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BaseCamp_Web_API.Api.Configuration;
using BaseCamp_Web_API.Api.Requests.Authentication;
using BaseCamp_Web_API.Api.Responses.Authentication;
using BaseCamp_Web_API.Api.ServiceAbstractions;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.Entities.Authentication;
using BaseCamp_WEB_API.Core.Enums;
using BaseCamp_WEB_API.Core.RepositoryAbstractions;
using BaseCamp_WEB_API.Data.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;

namespace BaseCamp_Web_API.Api.Services
{
    /// <inheritdoc/>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IMapper _mapper;
        private readonly JwtConfig _jwtConfig;
        private readonly TokenValidationParameters _tokenValidationParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="userRepository">For requests concerning users.</param>
        /// <param name="passwordHasher">For user password hashing.</param>
        /// <param name="mapper">For entities mapping.</param>
        /// <param name="optionsMonitor">For JWT-token generation.</param>
        /// <param name="tokenValidationParameters">For JWT-token validation.</param>
        /// <param name="refreshTokenRepository">For requests related with refresh tokens.</param>
        public UserService(IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            IMapper mapper,
            IOptionsMonitor<JwtConfig> optionsMonitor,
            TokenValidationParameters tokenValidationParameters,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _tokenValidationParameters = tokenValidationParameters;
            _refreshTokenRepository = refreshTokenRepository;
            _jwtConfig = optionsMonitor.CurrentValue;
        }

        /// <inheritdoc/>
        public async Task<AuthenticationResponse> RegisterAsync(RegisterUserRequest request)
        {
            var existingWithUsername = (await _userRepository.GetUserByUsernameAsync(request.Username)).FirstOrDefault();
            var existingWithEmail = (await _userRepository.GetUserByEmailAsync(request.Email)).FirstOrDefault();
            var errors = new List<string>();
            if (existingWithUsername != null)
            {
                errors.Add("User with this username already exists!");
            }

            if (existingWithEmail != null)
            {
                errors.Add("User with this email already exists!");
            }

            if (errors.Any())
            {
                return new AuthenticationResponse
                {
                    Success = false,
                    Errors = errors
                };
            }

            request.RoleId = UserRoles.Customer;
            var userToCreate = _mapper.Map<User>(request);
            userToCreate.HashedPassword = _passwordHasher.HashPassword(_mapper.Map<User>(request), request.Password);
            UserRepository.PutFieldsToUppercase(userToCreate);
            UserRepository.PutUsernameToLowercase(userToCreate);

            try
            {
                userToCreate.Id = await _userRepository.CreateAsync(userToCreate);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex);
                return new AuthenticationResponse
                {
                    Success = false,
                    Errors = new List<string>
                    {
                        "Database error occured!"
                    }
                };
            }

            return await GenerateAuthResultForUserAsync(userToCreate);
        }

        /// <inheritdoc/>
        public async Task<AuthenticationResponse> LoginAsync(LoginUserRequest request)
        {
            var existingUser = (await _userRepository.GetUserByUsernameAsync(request.Username)).FirstOrDefault();
            if (existingUser == null)
            {
                return new AuthenticationResponse
                {
                    Success = false,
                    Errors = new List<string>
                    {
                        "User with such username does not exist!"
                    }
                };
            }

            var passwordCorrect = IsPasswordCorrect(existingUser, request.Password);
            if (passwordCorrect)
            {
                return await GenerateAuthResultForUserAsync(existingUser);
            }

            return new AuthenticationResponse
            {
                Success = false,
                Errors = new List<string>
                {
                    "Check username/password combination!"
                }
            };
        }

        /// <inheritdoc/>
        public async Task<AuthenticationResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var validatedToken = GetPrincipalFromToken(request.Token);
            if (validatedToken == null)
            {
                return new AuthenticationResponse
                {
                    Errors = new List<string>
                    {
                        "Invalid token!"
                    }
                };
            }

            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = UnixTimeStampToDateTime(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthenticationResponse
                {
                    Success = false,
                    Errors = new List<string>
                    {
                        "Access token has not expired yet!"
                    }
                };
            }

            var storedRefreshToken = (await _refreshTokenRepository.GetByRefreshTokenValueAsync(request.RefreshToken)).FirstOrDefault();

            if (storedRefreshToken == null)
            {
                return new AuthenticationResponse
                {
                    Errors = new List<string>
                    {
                        "Refresh token does not exist!"
                    }
                };
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AuthenticationResponse
                {
                    Errors = new List<string>
                    {
                        "Refresh token has expired, user needs to re-login."
                    },
                    Success = false
                };
            }

            if (storedRefreshToken.IsRevoked)
            {
                return new AuthenticationResponse
                {
                    Errors = new List<string>
                    {
                        "Refresh token has been revoked!"
                    },
                    Success = false
                };
            }

            if (storedRefreshToken.IsUsed)
            {
                return new AuthenticationResponse
                {
                    Errors = new List<string>
                    {
                        "Refresh token has been used."
                    },
                    Success = false
                };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResponse
                {
                    Errors = new List<string>
                    {
                        "This refresh token does not match the saved token!"
                    },
                    Success = false
                };
            }

            storedRefreshToken.IsUsed = true;
            await _refreshTokenRepository.UpdateAsync(storedRefreshToken.Id, storedRefreshToken);

            var dbUser = (await _userRepository.GetByIdAsync(storedRefreshToken.UserId)).FirstOrDefault();
            return await GenerateAuthResultForUserAsync(dbUser);
        }

        /// <summary>
        /// Generates a JWT-token/refresh-token pair.
        /// </summary>
        /// <param name="user">User entity to get info from.</param>
        /// <returns></returns>
        private async Task<AuthenticationResponse> GenerateAuthResultForUserAsync(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
            var guid = Guid.NewGuid().ToString();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, guid),
                    new Claim(ClaimTypes.Sid, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.Add(_jwtConfig.TokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                IsUsed = false,
                IsRevoked = false,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                Token = RandomString(25) + Guid.NewGuid()
            };

            await _refreshTokenRepository.CreateAsync(refreshToken);

            return new AuthenticationResponse
            {
                Token = jwtToken,
                Success = true,
                RefreshToken = refreshToken.Token
            };
        }

        private bool IsPasswordCorrect(User user, string password)
        {
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.HashedPassword, password);

            return verificationResult == PasswordVerificationResult.Success;
        }

        private string RandomString(int length)
        {
            var random = new Random();
            var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable
                .Repeat(alphabet, length)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = jwtTokenHandler.ValidateToken(
                    token,
                    _tokenValidationParameters,
                    out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return validatedToken is JwtSecurityToken jwtSecurityToken &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512,
                       StringComparison.InvariantCultureIgnoreCase);
        }

        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dtDateTime;
        }
    }
}