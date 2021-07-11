using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using BaseCamp_Web_API.Api;
using BaseCamp_Web_API.Api.Abstractions.Services;
using BaseCamp_Web_API.Api.Configuration;
using BaseCamp_Web_API.Api.Services;
using BaseCamp_WEB_API.Core.DbContextAbstractions;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.Entities.Authentication;
using BaseCamp_WEB_API.Core.Entities.Authorization;
using BaseCamp_WEB_API.Core.Enums;
using BaseCamp_WEB_API.Data.Contexts;
using BaseCamp_WEB_API.Data.Repositories;
using BaseCamp_Web_API.Tests.Tests.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Moq;
using SqlKata.Execution;

namespace BaseCamp_Web_API.Tests.Fixtures.Services
{
    /// <summary>
    /// Contains needed resources and additional functionality for tests in <see cref="AuthServiceTests"/>.
    /// </summary>
    public class AuthServiceTestsFixture
    {
        /// <summary>
        /// <see cref="AuthService"/> instance for testing.
        /// </summary>
        public readonly IAuthService AuthSrvc;

        /// <summary>
        /// <see cref="TokenService"/> instance for testing.
        /// </summary>
        public readonly ITokenService TokenSrvc;

        /// <summary>
        /// Mapper.
        /// </summary>
        public readonly IMapper Mapper;

        /// <summary>
        /// Collection of users for testing.
        /// </summary>
        public IEnumerable<User> Users;

        /// <summary>
        /// Collection of refresh tokens for testing.
        /// </summary>
        public IEnumerable<RefreshToken> RefreshTokens;

        /// <summary>
        /// Configuration data for refresh tokens.
        /// </summary>
        public RefreshTokenConfig RefreshTokenCfg;

        /// <summary>
        /// Configuration data for JWT.
        /// </summary>
        public JwtConfig JwtCfg;

        /// <summary>
        /// Validation parameters for JWTs.
        /// </summary>
        public TokenValidationParameters TokenValidationParams;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthServiceTestsFixture"/> class.
        /// </summary>
        public AuthServiceTestsFixture()
        {
            var mapperConfiguration = new MapperConfiguration(opts => { opts.AddProfile<MappingProfile>(); });
            Mapper = mapperConfiguration.CreateMapper();

            var queryFactoryMock = new Mock<QueryFactory>();
            var userRepositoryMock = new Mock<UserRepository>(queryFactoryMock.Object);
            userRepositoryMock.Setup(r => r.GetUserByUsernameAsync(It.Is<string>(s =>
                    Regex.IsMatch(s, "^(?=.{4,20}$)(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$"))))
                .ReturnsAsync((string username) => GetUserByUsername(username));
            userRepositoryMock.Setup(r => r.GetUserByEmailAsync(It.Is<string>(s =>
                    Regex.IsMatch(s, "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$"))))
                .ReturnsAsync((string email) => GetUserByEmail(email));
            userRepositoryMock.Setup(r => r.GetByIdAsync(It.Is<int>(i => i > 0 && i < 4)))
                .ReturnsAsync((int id) => GetUserById(id));
            userRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(4);

            var refreshTokenRepositoryMock = new Mock<RefreshTokenRepository>(queryFactoryMock.Object);
            refreshTokenRepositoryMock.Setup(r => r.GetByRefreshTokenValueAsync(It.Is<string>(t => t.Length > 0)))
                .ReturnsAsync((string refreshToken) => GetRefreshTokenByValue(refreshToken));
            refreshTokenRepositoryMock.Setup(r => r.UpdateAsync(It.Is<int>(i => i > 0), It.IsAny<RefreshToken>()))
                .ReturnsAsync(1);
            refreshTokenRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<RefreshToken>()))
                .ReturnsAsync(1);

            JwtCfg = new JwtConfig
            {
                Secret = "uswlssoncpxvwpdpjinrswxqpefmrbgo",
                TokenLifetime = new TimeSpan(1, 0, 0)
            };

            RefreshTokenCfg = new RefreshTokenConfig { TokenLifetime = new TimeSpan(10, 0, 0) };

            var passwordHasher = new PasswordHasher<User>();

            TokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtCfg.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true
            };

            var userPolicyPairRecords = DbContextMockHelper.GetQueryableMockDbSet(GetUserPolicyPairs().ToList());
            var policyClaimPairRecords = DbContextMockHelper.GetQueryableMockDbSet(GetPolicyClaimPairs().ToList());
            var claimRecords = DbContextMockHelper.GetQueryableMockDbSet(GetClaims().ToList());

            var policyAuthorizationContextMock = new Mock<IPolicyAuthorizationDbContext>();
            policyAuthorizationContextMock.Setup(c => c.UserPolicyPairRecords).Returns(userPolicyPairRecords);
            policyAuthorizationContextMock.Setup(c => c.PolicyClaimPairRecords).Returns(policyClaimPairRecords);
            policyAuthorizationContextMock.Setup(c => c.ClaimRecords).Returns(claimRecords);

            TokenSrvc = new TokenService(JwtCfg, RefreshTokenCfg, refreshTokenRepositoryMock.Object, policyAuthorizationContextMock.Object);

            AuthSrvc = new AuthService(userRepositoryMock.Object,
                passwordHasher,
                Mapper,
                TokenValidationParams,
                refreshTokenRepositoryMock.Object,
                TokenSrvc);
        }

        /// <summary>
        /// Returns a collection of users for testing.
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
                    RoleId = UserRoles.Admin
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
        /// Resets users for testing to their initial state.
        /// </summary>
        public void ResetUsersCollection()
        {
            Users = GetAllUsers();
        }

        /// <summary>
        /// Returns a user by ID from users collection for testing.
        /// </summary>
        /// <param name="id">ID of a user.</param>
        /// <returns></returns>
        public IEnumerable<User> GetUserById(int id)
        {
            return Users.Where(u => u.Id == id);
        }

        /// <summary>
        /// Returns a user by e-mail from users collection for testing.
        /// </summary>
        /// <param name="email">E-mail of a user.</param>
        /// <returns></returns>
        public IEnumerable<User> GetUserByEmail(string email)
        {
            return Users.Where(u => string.Equals(u.Email, email));
        }

        /// <summary>
        /// Returns a user by username from users collection for testing.
        /// </summary>
        /// <param name="username">Username of a user.</param>
        /// <returns></returns>
        public IEnumerable<User> GetUserByUsername(string username)
        {
            return Users.Where(u => string.Equals(u.Username, username));
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

        /// <summary>
        /// Returns a collection of refresh tokens for testing.
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

        /// <summary>
        /// Resets a collection of refresh tokens to its initial state.
        /// </summary>
        public void ResetRefreshTokensCollection()
        {
            RefreshTokens = GetAllRefreshTokens();
        }

        /// <summary>
        /// Return a refresh token from refresh tokens collection by its value.
        /// </summary>
        /// <param name="refreshToken">Token value to look for.</param>
        /// <returns></returns>
        public IEnumerable<RefreshToken> GetRefreshTokenByValue(string refreshToken)
        {
            return RefreshTokens.Where(r => Equals(r.Token, refreshToken));
        }
    }
}