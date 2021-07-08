using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BaseCamp_Web_API.Api.Requests.Authentication;
using BaseCamp_Web_API.Api.Responses.Authentication;
using BaseCamp_Web_API.Api.ServiceAbstractions;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.Enums;
using BaseCamp_WEB_API.Core.RepositoryAbstractions;
using BaseCamp_WEB_API.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;

namespace BaseCamp_Web_API.Api.Services
{
    /// <summary>
    /// Contains functionality for authentication.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IMapper _mapper;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly ITokenService _tokenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthService"/> class.
        /// </summary>
        /// <param name="userRepository">For requests concerning users.</param>
        /// <param name="passwordHasher">For user password hashing.</param>
        /// <param name="mapper">For entities mapping.</param>
        /// <param name="tokenValidationParameters">For JWT-token validation.</param>
        /// <param name="refreshTokenRepository">For requests related with refresh tokens.</param>
        /// <param name="tokenService">Service for token generation.</param>
        public AuthService(IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            IMapper mapper,
            TokenValidationParameters tokenValidationParameters,
            IRefreshTokenRepository refreshTokenRepository,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _tokenValidationParameters = tokenValidationParameters;
            _refreshTokenRepository = refreshTokenRepository;
            _tokenService = tokenService;
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

            var userToCreate = _mapper.Map<User>(request);
            userToCreate.RoleId = UserRoles.Customer;
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

            return await _tokenService.GenerateTokenPairForUserAsync(userToCreate);
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
                return await _tokenService.GenerateTokenPairForUserAsync(existingUser);
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

            var storedRefreshToken = (await _refreshTokenRepository.GetByRefreshTokenValueAsync(request.RefreshToken)).SingleOrDefault();

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
            return await _tokenService.GenerateTokenPairForUserAsync(dbUser);
        }

        private bool IsPasswordCorrect(User user, string password)
        {
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.HashedPassword, password);

            return verificationResult == PasswordVerificationResult.Success;
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