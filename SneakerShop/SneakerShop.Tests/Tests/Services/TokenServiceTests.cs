using BaseCamp_Web_API.Api.Services;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_Web_API.Tests.Fixtures.Services;
using FluentAssertions;
using Xunit;

namespace BaseCamp_Web_API.Tests.Tests.Services
{
    /// <summary>
    /// Contains unit tests for <see cref="TokenService"/>.
    /// </summary>
    public class TokenServiceTests : IClassFixture<TokenServiceTestsFixture>
    {
        private TokenServiceTestsFixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenServiceTests"/> class.
        /// </summary>
        /// /// <param name="fixture">Contains resources for tests.</param>
        public TokenServiceTests(TokenServiceTestsFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Tests service response when providing a register request with duplicate username.
        /// </summary>
        [Fact]
        public async void TokenService_GeneratingTokenPair_SuccessfulAuthenticationResponseWithAPairOfTokens()
        {
            // Arrange
            var user = new User
            {
                Email = "bingus@test.com",
                Id = 5
            };

            // Act
            var response = await _fixture.TokenSrvc.GenerateTokenPairForUserAsync(user);

            // Assert
            response.Errors.Should().BeNull();
            response.Success.Should().BeTrue();
            response.Token.Should().NotBeEmpty();
            response.RefreshToken.Should().NotBeEmpty();
        }
    }
}