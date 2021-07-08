using System;
using System.Collections.Generic;
using System.Linq;
using BaseCamp_Web_API.Api.Controllers;
using BaseCamp_Web_API.Api.Requests.Authentication;
using BaseCamp_Web_API.Api.Responses.Authentication;
using BaseCamp_Web_API.Api.ServiceAbstractions;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.Entities.Authentication;
using BaseCamp_WEB_API.Core.Enums;
using BaseCamp_Web_API.Tests.Tests.Controllers;
using Moq;

namespace BaseCamp_Web_API.Tests.Fixtures.Controllers
{
    /// <summary>
    /// Contains needed resources and additional functionality for tests in <see cref="AuthControllerTests"/>.
    /// </summary>
    public class AuthControllerTestsFixture
    {
        /// <summary>
        /// Controller instance for testing.
        /// </summary>
        public readonly AuthController Controller;

        /// <summary>
        /// Collection of users for testing.
        /// </summary>
        public IEnumerable<User> Users;

        /// <summary>
        /// Collection of refresh tokens for testing.
        /// </summary>
        public IEnumerable<RefreshToken> RefreshTokens;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthControllerTestsFixture"/> class.
        /// </summary>
        public AuthControllerTestsFixture()
        {
            Users = GetAllUsers();
            RefreshTokens = GetAllRefreshTokens();

            var authServiceMock = new Mock<IAuthService>();
            authServiceMock.Setup(s => s.RegisterAsync(It.Is<RegisterUserRequest>(r => Users.Any(u => r.Username != u.Username))))
                .ReturnsAsync(new RegisterUserResponse { Success = true });
            authServiceMock.Setup(s => s.RegisterAsync(It.Is<RegisterUserRequest>(r => Users.Count(u => u.Username == r.Username) != 0)))
                .ReturnsAsync(new RegisterUserResponse { Success = false });

            authServiceMock.Setup(s => s.LoginAsync(It.Is<LoginUserRequest>(r => Users.Any(u => r.Username != u.Username))))
                .ReturnsAsync(new AuthenticationResponse { Success = false });
            authServiceMock.Setup(s => s.LoginAsync(It.Is<LoginUserRequest>(r => Users.Any(u => r.Username == u.Username))))
                .ReturnsAsync(new AuthenticationResponse { Success = true });

            authServiceMock.Setup(s => s.RefreshTokenAsync(It.Is<RefreshTokenRequest>(r => RefreshTokens.Any(t => t.Token != r.RefreshToken))))
                .ReturnsAsync(new AuthenticationResponse { Success = false });
            authServiceMock.Setup(s => s.RefreshTokenAsync(It.Is<RefreshTokenRequest>(r => RefreshTokens.Any(t => t.Token == r.RefreshToken))))
                .ReturnsAsync(new AuthenticationResponse { Success = true });

            Controller = new AuthController(authServiceMock.Object);
        }

        /// <summary>
        /// Return a collection of initial users collection.
        /// </summary>
        /// <returns>Collection of users.</returns>
        public IEnumerable<User> GetAllUsers()
        {
            return new List<User>
            {
                new ()
                {
                    Username = "testuser",
                    HashedPassword =
                        "AQAAAAEAACcQAAAAEB1u4UQNmXPOKyIy6ccBaC1Y/LKz2wNIxdy1ZEbCo5nucrFqnBbrJW/pZHEWoSOUug==",
                    Email = "string@test.com",
                    FirstName = "ABOBA",
                    LastName = "ABOBIX",
                    Patronymic = "ABOBOVYCH",
                    Age = 69,
                    Id = 1,
                    GenderId = 1,
                    RoleId = UserRoles.Customer
                },
                new ()
                {
                    Username = "tokechu",
                    HashedPassword =
                        "AQAAAAEAACcQAAAAEB1u4UQNmXPOKyIy6ccBaC1Y/LKz2wNIxdy1ZEbCo5nucrFqnBbrJW/pZHEWoSOUug==",
                    Email = "test@string.com",
                    FirstName = "OBEME",
                    LastName = "OBEMIX",
                    Patronymic = "OBEMOVYCH",
                    Age = 47,
                    Id = 2,
                    GenderId = 2,
                    RoleId = UserRoles.Customer
                },
                new ()
                {
                    Username = "test",
                    HashedPassword =
                        "AQAAAAEAACcQAAAAEB1u4UQNmXPOKyIy6ccBaC1Y/LKz2wNIxdy1ZEbCo5nucrFqnBbrJW/pZHEWoSOUug==",
                    Email = "string@test.com",
                    FirstName = "JOHN",
                    LastName = "DOE",
                    Patronymic = "MYKOLAYOVYCH",
                    Age = 21,
                    Id = 3,
                    GenderId = 1,
                    RoleId = UserRoles.Customer
                }
            };
        }

        /// <summary>
        /// Resets collection of users to its initial state.
        /// </summary>
        public void ResetUsersCollection()
        {
            Users = GetAllUsers();
        }

        /// <summary>
        /// Return a collection of initial refresh tokens collection.
        /// </summary>
        /// <returns>Collection of refresh tokens.</returns>
        public IEnumerable<RefreshToken> GetAllRefreshTokens()
        {
            return new List<RefreshToken>
            {
                new ()
                {
                    Id = 1,
                    Token = "0KAO71PRKM22SQ68G8T7UUQNE13990afe-3199-468d-92de-d47e9d873b19",
                    CreationDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddMonths(6),
                    IsRevoked = false,
                    IsUsed = false,
                    JwtId = "2b503899-a279-46c4-9431-8c74f6c76ed5",
                    UserId = 1
                }
            };
        }
    }
}