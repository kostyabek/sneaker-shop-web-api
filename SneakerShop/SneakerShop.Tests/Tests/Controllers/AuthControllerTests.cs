using BaseCamp_Web_API.Api.Requests.Authentication;
using BaseCamp_Web_API.Api.Responses.Authentication;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_Web_API.Tests.Fixtures.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
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
            _fixture.ResetUsersCollection();
        }

        /// <summary>
        /// Tests controller response when registering a new <see cref="User"/> with invalid request.
        /// </summary>
        [Fact]
        public async void AuthController_RegisteringNewUser_BadRequestObjectResultWithBadAuthenticationResponse()
        {
            // Arrange
            var request = new RegisterUserRequest
            {
                Username = "testuser"
            };

            // Act
            var response = await _fixture.Controller.RegisterAsync(request);

            // Assert
            response.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeEquivalentTo(new RegisterUserResponse { Success = false });
        }

        /// <summary>
        /// Tests controller response when registering a new <see cref="User"/> with valid request.
        /// </summary>
        [Fact]
        public async void AuthController_RegisteringNewUser_OkObjectResultWithSuccessfulAuthenticationResponse()
        {
            // Arrange
            var request = new RegisterUserRequest
            {
                Username = "newuser"
            };

            // Act
            var response = await _fixture.Controller.RegisterAsync(request);

            // Assert
            response.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(new RegisterUserResponse { Success = true });
        }

        /// <summary>
        /// Tests controller response when logging in a <see cref="User"/> with invalid request.
        /// </summary>
        [Fact]
        public async void AuthController_UserLogsIn_BadRequestObjectResultWithBadAuthenticationResponse()
        {
            // Arrange
            var request = new LoginUserRequest
            {
                Username = "newuser"
            };

            // Act
            var response = await _fixture.Controller.LoginAsync(request);

            // Assert
            response.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeEquivalentTo(new AuthenticationResponse { Success = false });
        }

        /// <summary>
        /// Tests controller response when logging in a <see cref="User"/> with valid request.
        /// </summary>
        [Fact]
        public async void AuthController_UserLogsIn_OkObjectResultWithSuccessfulAuthenticationResponse()
        {
            // Arrange
            var request = new LoginUserRequest
            {
                Username = "testuser"
            };

            // Act
            var response = await _fixture.Controller.LoginAsync(request);

            // Assert
            response.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(new AuthenticationResponse { Success = true });
        }

        /// <summary>
        /// Tests controller response when user refreshes his tokens with invalid request.
        /// </summary>
        [Fact]
        public async void AuthController_UserRefreshesTokens_BadRequestObjectResultWithBadAuthenticationResponse()
        {
            // Arrange
            var request = new RefreshTokenRequest
            {
                RefreshToken = "0KAO71PRKM22SQ68G8T7UUQNE13990afe-1234-468d-92de-d47e9d873b19"
            };

            // Act
            var response = await _fixture.Controller.RefreshTokenAsync(request);

            // Assert
            response.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().BeEquivalentTo(new AuthenticationResponse { Success = false });
        }

        /// <summary>
        /// Tests controller response when user refreshes his tokens with valid request.
        /// </summary>
        [Fact]
        public async void AuthController_UserRefreshesTokens_OkObjectResultWithSuccessfulAuthenticationResponse()
        {
            // Arrange
            var request = new RefreshTokenRequest
            {
                RefreshToken = "0KAO71PRKM22SQ68G8T7UUQNE13990afe-3199-468d-92de-d47e9d873b19"
            };

            // Act
            var response = await _fixture.Controller.RefreshTokenAsync(request);

            // Assert
            response.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(new AuthenticationResponse { Success = true });
        }
    }
}