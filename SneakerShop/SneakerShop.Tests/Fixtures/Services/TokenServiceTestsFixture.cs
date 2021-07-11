using System;
using System.Collections.Generic;
using System.Linq;
using BaseCamp_Web_API.Api.Abstractions.Services;
using BaseCamp_Web_API.Api.Configuration;
using BaseCamp_Web_API.Api.Services;
using BaseCamp_WEB_API.Core.DbContextAbstractions;
using BaseCamp_WEB_API.Core.Entities.Authentication;
using BaseCamp_WEB_API.Core.Entities.Authorization;
using BaseCamp_WEB_API.Data.Repositories;
using BaseCamp_Web_API.Tests.Tests.Services;
using Moq;
using SqlKata.Execution;

namespace BaseCamp_Web_API.Tests.Fixtures.Services
{
    /// <summary>
    /// Contains needed resources and additional functionality for tests in <see cref="TokenServiceTests"/>.
    /// </summary>
    public class TokenServiceTestsFixture
    {
        /// <summary>
        /// Instance of <see cref="TokenService"/> for testing.
        /// </summary>
        public readonly ITokenService TokenSrvc;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenServiceTestsFixture"/> class.
        /// </summary>
        public TokenServiceTestsFixture()
        {
            var jwtConfig = new JwtConfig
            {
                Secret = "uswlssoncpxvwpdpjinrswxqpefmrbgo",
                TokenLifetime = new TimeSpan(1, 0, 0)
            };

            var refreshTokenConfig = new RefreshTokenConfig { TokenLifetime = new TimeSpan(10, 0, 0) };

            var queryFactoryMock = new Mock<QueryFactory>();
            var refreshTokenRepositoryMock = new Mock<RefreshTokenRepository>(queryFactoryMock.Object);
            refreshTokenRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<RefreshToken>()))
                .ReturnsAsync(1);

            var userPolicyPairRecords = DbContextMockHelper.GetQueryableMockDbSet(GetUserPolicyPairs().ToList());
            var policyClaimPairRecords = DbContextMockHelper.GetQueryableMockDbSet(GetPolicyClaimPairs().ToList());
            var claimRecords = DbContextMockHelper.GetQueryableMockDbSet(GetClaims().ToList());

            var policyAuthorizationContextMock = new Mock<IPolicyAuthorizationDbContext>();
            policyAuthorizationContextMock.Setup(c => c.UserPolicyPairRecords).Returns(userPolicyPairRecords);
            policyAuthorizationContextMock.Setup(c => c.PolicyClaimPairRecords).Returns(policyClaimPairRecords);
            policyAuthorizationContextMock.Setup(c => c.ClaimRecords).Returns(claimRecords);

            TokenSrvc = new TokenService(jwtConfig, refreshTokenConfig, refreshTokenRepositoryMock.Object, policyAuthorizationContextMock.Object);
        }

        /// <summary>
        /// Returns a collection of policy ID and user ID it is attached to for testing.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserPolicyPair> GetUserPolicyPairs()
        {
            return new List<UserPolicyPair>
            {
                new () { Id = 1, PolicyId = 1, UserId = 1 }
            };
        }

        /// <summary>
        /// Returns a collection of policy ID and claim ID it is attached to for testing.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PolicyClaimPair> GetPolicyClaimPairs()
        {
            return new List<PolicyClaimPair>
            {
                new () { Id = 1, PolicyId = 1, ClaimId = 1 }
            };
        }

        /// <summary>
        /// Returns a collection of policy ID and user ID it is attached to for testing.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ClaimRecord> GetClaims()
        {
            return new List<ClaimRecord>
            {
                new () { Id = 1, Type = "CanReadUsers", Value = "1" }
            };
        }
    }
}