using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BaseCamp_Web_API.Api.Configuration;
using BaseCamp_Web_API.Api.Responses.Authentication;
using BaseCamp_Web_API.Api.ServiceAbstractions;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.Entities.Authentication;
using BaseCamp_WEB_API.Core.RepositoryAbstractions;
using BaseCamp_WEB_API.Data.Contexts;
using Microsoft.IdentityModel.Tokens;

namespace BaseCamp_Web_API.Api.Services
{
    /// <summary>
    /// Contains functionality for token generation.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly JwtConfig _jwtConfig;
        private readonly RefreshTokenConfig _refreshTokenConfig;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly PolicyAuthorizationContext _policyAuthorizationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenService"/> class.
        /// </summary>
        /// <param name="jwtConfig">JWT configuration data.</param>
        /// <param name="refreshTokenRepository">For requests related with refresh tokens.</param>
        /// <param name="refreshTokenConfig">Refresh token configuration data.</param>
        /// <param name="policyAuthorizationContext">For accessing data about user policies.</param>
        public TokenService(JwtConfig jwtConfig,
            RefreshTokenConfig refreshTokenConfig,
            IRefreshTokenRepository refreshTokenRepository,
            PolicyAuthorizationContext policyAuthorizationContext)
        {
            _jwtConfig = jwtConfig;
            _refreshTokenConfig = refreshTokenConfig;
            _refreshTokenRepository = refreshTokenRepository;
            _policyAuthorizationContext = policyAuthorizationContext;
        }

        /// <inheritdoc/>
        public async Task<AuthenticationResponse> GenerateTokenPairForUserAsync(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
            var guid = Guid.NewGuid().ToString();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(ClaimTypes.Sid, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.RoleId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, guid)
                }),
                Expires = DateTime.UtcNow.Add(_jwtConfig.TokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var userPolicyPairs = _policyAuthorizationContext
                .UserPolicyPairRecords.Where(r => r.UserId == user.Id)
                .ToList();
            foreach (var userPolicyPair in userPolicyPairs)
            {
                var policyClaimPairs = _policyAuthorizationContext
                    .PolicyClaimPairRecords.Where(r => r.PolicyId == userPolicyPair.PolicyId)
                    .ToList();
                foreach (var policyClaimPair in policyClaimPairs)
                {
                    var claim = _policyAuthorizationContext
                        .ClaimRecords.FirstOrDefault(r => r.Id == policyClaimPair.ClaimId);
                    if (claim != null)
                    {
                        ((List<Claim>)tokenDescriptor.Subject.Claims).Add(new Claim(claim.Type, claim.Value));
                    }
                }
            }

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                IsUsed = false,
                IsRevoked = false,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.Add(_refreshTokenConfig.TokenLifetime),
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

        /// <inheritdoc/>
        public string RandomString(int length)
        {
            var random = new Random();
            var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable
                .Repeat(alphabet, length)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());
        }
    }
}