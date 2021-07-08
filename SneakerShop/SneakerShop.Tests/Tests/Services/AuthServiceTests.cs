using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using BaseCamp_Web_API.Api.Requests.Authentication;
using BaseCamp_Web_API.Api.Services;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.Entities.Authentication;
using BaseCamp_WEB_API.Core.Enums;
using BaseCamp_Web_API.Tests.Fixtures.Services;
using FluentAssertions;
using Xunit;

namespace BaseCamp_Web_API.Tests.Tests.Services
{
    /// <summary>
    /// Contains unit tests for <see cref="AuthService"/>.
    /// </summary>
    public class AuthServiceTests : IClassFixture<AuthServiceTestsFixture>
    {
        private readonly AuthServiceTestsFixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthServiceTests"/> class.
        /// </summary>
        /// <param name="fixture">Contains resources for tests.</param>
        public AuthServiceTests(AuthServiceTestsFixture fixture)
        {
            _fixture = fixture;
            _fixture.ResetUsersCollection();
            _fixture.ResetRefreshTokensCollection();
            _fixture.JwtCfg.TokenLifetime = new TimeSpan(1, 0, 0);
            _fixture.RefreshTokenCfg.TokenLifetime = new TimeSpan(1, 0, 0, 0);
        }

        /// <summary>
        /// Tests service response when providing a register request with duplicate username.
        /// </summary>
        [Fact]
        public async void AuthService_RegisteringNewUser_AuthenticationResponseWithMessageAboutDuplicateUsername()
        {
            // Arrange
            var request = new RegisterUserRequest
            {
                Username = "testuser",
                Password = "Nnnnn1234!",
                Email = "bingus@test.com",
                FirstName = "ABOBA",
                LastName = "ABOBIX",
                Patronymic = "ABOBOVYCH",
                Age = 69,
                GenderId = 1
            };
            var usersNumberBefore = _fixture.Users.Count();

            // Act
            var response = await _fixture.AuthSrvc.RegisterAsync(request);

            // Assert
            _fixture.Users.Count().Should().Be(usersNumberBefore);
            response.Success.Should().BeFalse();
            response.Errors.Should().ContainSingle();
            response.Errors.ToList()[0].Should().Be("User with this username already exists!");
        }

        /// <summary>
        /// Tests service response when providing a register request with duplicate email.
        /// </summary>
        [Fact]
        public async void AuthService_RegisteringNewUser_AuthenticationResponseWithMessageAboutDuplicateEmail()
        {
            // Arrange
            var request = new RegisterUserRequest
            {
                Username = "usernamed",
                Password = "Nnnnn1234!",
                Email = "string@test.com",
                FirstName = "ABOBA",
                LastName = "ABOBIX",
                Patronymic = "ABOBOVYCH",
                Age = 69,
                GenderId = 1
            };
            var usersNumberBefore = _fixture.Users.Count();

            // Act
            var response = await _fixture.AuthSrvc.RegisterAsync(request);

            // Assert
            _fixture.Users.Count().Should().Be(usersNumberBefore);
            response.Success.Should().BeFalse();
            response.Errors.Should().ContainSingle();
            response.Errors.ToList()[0].Should().Be("User with this email already exists!");
        }

        /// <summary>
        /// Tests service response when providing a register request with duplicate username and email.
        /// </summary>
        [Fact]
        public async void AuthService_RegisteringNewUser_AuthenticationResponseWithMessageAboutDuplicateUsernameAndEmail()
        {
            // Arrange
            var request = new RegisterUserRequest
            {
                Username = "testuser",
                Password = "Nnnnn1234!",
                Email = "string@test.com",
                FirstName = "ABOBA",
                LastName = "ABOBIX",
                Patronymic = "ABOBOVYCH",
                Age = 69,
                GenderId = 1
            };
            var usersNumberBefore = _fixture.Users.Count();

            // Act
            var response = await _fixture.AuthSrvc.RegisterAsync(request);

            // Assert
            _fixture.Users.Count().Should().Be(usersNumberBefore);
            response.Success.Should().BeFalse();
            response.Errors.Count().Should().Be(2);
            response.Errors.ToList()[0].Should().Be("User with this username already exists!");
            response.Errors.ToList()[1].Should().Be("User with this email already exists!");
        }

        /// <summary>
        /// Tests service response when providing a register request with valid data.
        /// </summary>
        [Fact]
        public async void AuthService_RegisteringNewUser_SuccessfulAuthenticationResponseWithJwtAndRefreshTokenPair()
        {
            // Arrange
            var request = new RegisterUserRequest
            {
                Username = "usernamed",
                Password = "Nnnnn1234!",
                Email = "string-bing@test.com",
                FirstName = "ABOBA",
                LastName = "ABOBIX",
                Patronymic = "ABOBOVYCH",
                Age = 69,
                GenderId = 1
            };
            var usersNumberBefore = _fixture.Users.Count();

            // Act
            var response = await _fixture.AuthSrvc.RegisterAsync(request);
            var userToAdd = _fixture.Mapper.Map<User>(request);
            userToAdd.RoleId = UserRoles.Customer;
            ((List<User>)_fixture.Users).Add(userToAdd);

            // Assert
            _fixture.Users.Count().Should().Be(usersNumberBefore + 1);
            response.Success.Should().BeTrue();
            response.Errors.Should().BeNull();
            response.Token.Should().NotBeEmpty();
            response.RefreshToken.Should().NotBeEmpty();
        }

        /// <summary>
        /// Tests service response when providing a log-in request with non existing username.
        /// </summary>
        [Fact]
        public async void AuthService_UserLogsIn_AuthenticationResponseWithUsernameDoesNotExistError()
        {
            // Arrange
            var request = new LoginUserRequest
            {
                Username = "squidward",
                Password = "azerty123"
            };
            var refreshTokensNumberBefore = _fixture.RefreshTokens.Count();

            // Act
            var response = await _fixture.AuthSrvc.LoginAsync(request);

            // Assert
            _fixture.RefreshTokens.Count().Should().Be(refreshTokensNumberBefore);
            response.Success.Should().BeFalse();
            response.Errors.Should().ContainSingle();
            response.Errors.ToList()[0].Should().Be("User with such username does not exist!");
        }

        /// <summary>
        /// Tests service response when providing a log-in request with wrong credentials.
        /// </summary>
        [Fact]
        public async void AuthService_UserLogsIn_AuthenticationResponseWithWrongCredentialsError()
        {
            // Arrange
            var request = new LoginUserRequest
            {
                Username = "testuser",
                Password = "azerty123"
            };
            var refreshTokensNumberBefore = _fixture.RefreshTokens.Count();

            // Act
            var response = await _fixture.AuthSrvc.LoginAsync(request);

            // Assert
            _fixture.RefreshTokens.Count().Should().Be(refreshTokensNumberBefore);
            response.Success.Should().BeFalse();
            response.Errors.Should().ContainSingle();
            response.Errors.ToList()[0].Should().Be("Check username/password combination!");
        }

        /// <summary>
        /// Tests service response when providing a log-in request with valid credentials.
        /// </summary>
        [Fact]
        public async void AuthService_UserLogsIn_SuccessfulAuthenticationResponseWithJwtAndRefreshTokenPair()
        {
            // Arrange
            var request = new LoginUserRequest
            {
                Username = "testuser",
                Password = "Nnnnn1234!"
            };
            var refreshTokensNumberBefore = _fixture.RefreshTokens.Count();

            // Act
            var response = await _fixture.AuthSrvc.LoginAsync(request);
            ((List<RefreshToken>)_fixture.RefreshTokens).Add(new RefreshToken { Token = response.RefreshToken });

            // Assert
            _fixture.RefreshTokens.Count().Should().Be(refreshTokensNumberBefore + 1);
            response.Success.Should().BeTrue();
            response.Errors.Should().BeNull();
            response.Token.Should().NotBeEmpty();
            response.RefreshToken.Should().NotBeEmpty();
        }

        /// <summary>
        /// Tests service response when refreshing tokens with invalid JWT.
        /// </summary>
        [Fact]
        public async void AuthService_UserRefreshesTokens_AuthenticationResponseWithInvalidTokenError()
        {
            // Arrange
            var request = new RefreshTokenRequest
            {
                Token = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXXXJ9.eyJzdWIiOiJTVFJJTkdAU1RSSU5HRVIuQ09NIiwiZW1haWwiOiJTVFJJTkdAU1RSSU5HRVIuQ09NIiwianRpIjoiMmI1MDM4OTktYTI3OS00NmM0LTk0MzEtOGM3NGY2Yzc2ZWQ1IiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvc2lkIjoiMTUiLCJuYmYiOjE2MjQ4ODAwNTIsImV4cCI6MTYyNDg4MzY1MiwiaWF0IjoxNjI0ODgwMDUyfQ._GpOkXI0sJ8yeKXSoYnyR3qcK6sDNXWliq6ztuBWabUNsxdfcPIuarjlDxjLdS843XBxocdQlvFYlfX7JsmosA",
                RefreshToken = "0KAO71PRKM22SQ68G8T7UUQNE13990afe-3199-468d-92de-d47e9d873b19"
            };

            // Act
            var response = await _fixture.AuthSrvc.RefreshTokenAsync(request);

            // Assert
            _fixture.GetRefreshTokenByValue(request.RefreshToken)
                .SingleOrDefault()
                .IsUsed.Should().BeFalse();
            response.Success.Should().BeFalse();
            response.Errors.Should().ContainSingle();
            response.Errors.ToList()[0].Should().Be("Invalid token!");
        }

        /// <summary>
        /// Tests service response when refreshing tokens with non-expired JWT.
        /// </summary>
        [Fact]
        public async void AuthService_UserRefreshesTokens_AuthenticationResponseWithAccessTokenHasNotExpiredError()
        {
            // Arrange
            var authResponse = await _fixture.TokenSrvc.GenerateTokenPairForUserAsync(_fixture.Users.First());
            var request = new RefreshTokenRequest
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            };
            ((List<RefreshToken>)_fixture.RefreshTokens).Add(new RefreshToken { Token = request.RefreshToken });

            // Act
            var response = await _fixture.AuthSrvc.RefreshTokenAsync(request);

            // Assert
            _fixture.GetRefreshTokenByValue(request.RefreshToken)
                .SingleOrDefault()
                .IsUsed.Should().BeFalse();
            response.Success.Should().BeFalse();
            response.Errors.Should().ContainSingle();
            response.Errors.ToList()[0].Should().Be("Access token has not expired yet!");
        }

        /// <summary>
        /// Tests service response when refreshing tokens with non-existent refresh token.
        /// </summary>
        [Fact]
        public async void AuthService_UserRefreshesTokens_AuthenticationResponseWithRefreshTokenDoesNotExistError()
        {
            // Arrange
            _fixture.JwtCfg.TokenLifetime = new TimeSpan(0, 0, 0, 0, 1);
            var authResponse = await _fixture.TokenSrvc.GenerateTokenPairForUserAsync(_fixture.Users.First());
            var request = new RefreshTokenRequest
            {
                Token = authResponse.Token,
                RefreshToken = "0KAO71PRKM22SQ68G8T7UUQNE13990afe-3200-468d-92de-d47e9d873b19"
            };

            // Act
            var response = await _fixture.AuthSrvc.RefreshTokenAsync(request);

            // Assert
            response.Success.Should().BeFalse();
            response.Errors.Should().ContainSingle();
            response.Errors.ToList()[0].Should().Be("Refresh token does not exist!");
        }

        /// <summary>
        /// Tests service response when refreshing tokens with non-expired refresh token.
        /// </summary>
        [Fact]
        public async void AuthService_UserRefreshesTokens_AuthenticationResponseWithRefreshTokenHasExpiredError()
        {
            // Arrange
            _fixture.JwtCfg.TokenLifetime = new TimeSpan(0, 0, 0, 0, 1);
            _fixture.RefreshTokenCfg.TokenLifetime = new TimeSpan(0, 0, 0, 0, 1);
            var authResponse = await _fixture.TokenSrvc.GenerateTokenPairForUserAsync(_fixture.Users.First());
            var request = new RefreshTokenRequest
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            };
            ((List<RefreshToken>)_fixture.RefreshTokens).Add(new RefreshToken { Token = request.RefreshToken });

            // Act
            var response = await _fixture.AuthSrvc.RefreshTokenAsync(request);

            // Assert
            response.Success.Should().BeFalse();
            response.Errors.Should().ContainSingle();
            response.Errors.ToList()[0].Should().Be("Refresh token has expired, user needs to re-login.");
        }

        /// <summary>
        /// Tests service response when refreshing tokens with revoked refresh token.
        /// </summary>
        [Fact]
        public async void AuthService_UserRefreshesTokens_AuthenticationResponseWithRevokedRefreshTokenError()
        {
            // Arrange
            _fixture.JwtCfg.TokenLifetime = new TimeSpan(0, 0, 0, 0, 1);
            var authResponse = await _fixture.TokenSrvc.GenerateTokenPairForUserAsync(_fixture.Users.First());
            var request = new RefreshTokenRequest
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            };
            ((List<RefreshToken>)_fixture.RefreshTokens).Add(new RefreshToken
            {
                Token = request.RefreshToken,
                IsRevoked = true,
                ExpiryDate = DateTime.UtcNow.AddDays(1)
            });

            // Act
            var response = await _fixture.AuthSrvc.RefreshTokenAsync(request);

            // Assert
            response.Success.Should().BeFalse();
            response.Errors.Should().ContainSingle();
            response.Errors.ToList()[0].Should().Be("Refresh token has been revoked!");
        }

        /// <summary>
        /// Tests service response when refreshing tokens with used refresh token.
        /// </summary>
        [Fact]
        public async void AuthService_UserRefreshesTokens_AuthenticationResponseWithUsedRefreshTokenError()
        {
            // Arrange
            _fixture.JwtCfg.TokenLifetime = new TimeSpan(0, 0, 0, 0, 1);
            var authResponse = await _fixture.TokenSrvc.GenerateTokenPairForUserAsync(_fixture.Users.First());
            var request = new RefreshTokenRequest
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            };
            ((List<RefreshToken>)_fixture.RefreshTokens).Add(new RefreshToken
            {
                Token = request.RefreshToken,
                IsUsed = true,
                ExpiryDate = DateTime.UtcNow.AddDays(1)
            });

            // Act
            var response = await _fixture.AuthSrvc.RefreshTokenAsync(request);

            // Assert
            response.Success.Should().BeFalse();
            response.Errors.Should().ContainSingle();
            response.Errors.ToList()[0].Should().Be("Refresh token has been used.");
        }

        /// <summary>
        /// Tests service response when refreshing tokens with invalid refresh token.
        /// </summary>
        [Fact]
        public async void AuthService_UserRefreshesTokens_AuthenticationResponseWithRefreshTokenDoesNotMatchTheSavedOneError()
        {
            // Arrange
            _fixture.JwtCfg.TokenLifetime = new TimeSpan(0, 0, 0, 0, 200);
            var authResponse = await _fixture.TokenSrvc.GenerateTokenPairForUserAsync(_fixture.Users.First());
            var request = new RefreshTokenRequest
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            };
            ((List<RefreshToken>)_fixture.RefreshTokens).Add(new RefreshToken
            {
                Token = request.RefreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(1),
                JwtId = "2b503899-a279-46c4-1234-8c74f6c76ed5"
            });

            // Act
            var response = await _fixture.AuthSrvc.RefreshTokenAsync(request);

            // Assert
            response.Success.Should().BeFalse();
            response.Errors.Should().ContainSingle();
            response.Errors.ToList()[0].Should().Be("This refresh token does not match the saved token!");
        }

        /// <summary>
        /// Tests service response when refreshing tokens with valid data.
        /// </summary>
        [Fact]
        public async void AuthService_UserRefreshesTokens_SuccessfulAuthenticationResponseWithJwtAndRefreshTokenPair()
        {
            // Arrange
            _fixture.JwtCfg.TokenLifetime = new TimeSpan(0, 0, 0, 0, 100);
            var authResponse = await _fixture.TokenSrvc.GenerateTokenPairForUserAsync(_fixture.Users.First());
            var request = new RefreshTokenRequest
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            };

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var principal = jwtTokenHandler.ValidateToken(
                authResponse.Token,
                _fixture.TokenValidationParams,
                out var validatedToken1);

            ((List<RefreshToken>)_fixture.RefreshTokens).Add(new RefreshToken
            {
                Token = request.RefreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(1),
                JwtId = principal.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value,
                UserId = ((List<User>)_fixture.Users).FirstOrDefault().Id
            });

            // Act
            var response = await _fixture.AuthSrvc.RefreshTokenAsync(request);

            // Assert
            response.Success.Should().BeTrue();
            response.Errors.Should().BeNull();

            response.Token.Should().NotBeEmpty();
            principal = jwtTokenHandler.ValidateToken(
                response.Token,
                _fixture.TokenValidationParams,
                out var validatedToken2);
            var newTokenJti = principal.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
            _fixture.RefreshTokens.FirstOrDefault(t => t.JwtId == newTokenJti).Should().BeNull();

            response.RefreshToken.Should().NotBeEmpty();
            response.RefreshToken.Should().NotBe(request.RefreshToken);
        }
    }
}