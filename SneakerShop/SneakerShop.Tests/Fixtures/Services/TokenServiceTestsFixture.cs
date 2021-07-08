using System;
using BaseCamp_Web_API.Api.Configuration;
using BaseCamp_Web_API.Api.ServiceAbstractions;
using BaseCamp_Web_API.Api.Services;
using BaseCamp_WEB_API.Core.Entities.Authentication;
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

            TokenSrvc = new TokenService(jwtConfig, refreshTokenConfig, refreshTokenRepositoryMock.Object);
        }
    }
}