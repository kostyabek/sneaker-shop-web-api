using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_Web_API.Tests.Fixtures.Controllers;
using Xunit;

namespace BaseCamp_Web_API.Tests.Tests.Controllers
{
    /// <summary>
    /// Contains unit tests for AuthController.
    /// </summary>
    public class AuthControllerTests : IClassFixture<AuthControllerTestsFixture>
    {
        private readonly AuthControllerTestsFixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthControllerTests"/> class.
        /// </summary>
        /// <param name="fixture">Contains resources for tests.</param>
        public AuthControllerTests(AuthControllerTestsFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Tests controller response when registering a new <see cref="User"/> .
        /// </summary>
        [Fact]
        public async void AuthController_RegisteringNewUser_OkObjectResultWithAuthResponse()
        {
            // Arrange

            // Act

            // Assert
        }
    }
}