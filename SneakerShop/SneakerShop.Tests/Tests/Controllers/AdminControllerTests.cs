using System;
using System.Collections.Generic;
using System.Linq;
using BaseCamp_Web_API.Api.Controllers;
using BaseCamp_Web_API.Api.Requests.Users;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.Filters;
using BaseCamp_Web_API.Tests.Fixtures.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BaseCamp_Web_API.Tests.Tests.Controllers
{
    /// <summary>
    /// Contains unit tests for <see cref="AdminController"/>.
    /// </summary>
    public class AdminControllerTests : IClassFixture<AdminControllerTestsFixture>
    {
        private readonly AdminControllerTestsFixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminControllerTests"/> class.
        /// </summary>
        /// <param name="fixture">Contains resources for tests.</param>
        public AdminControllerTests(AdminControllerTestsFixture fixture)
        {
            _fixture = fixture;
            _fixture.ResetOrderCollection();
            _fixture.ResetUsersCollection();
            _fixture.ResetOrderSneakerCollection();
        }

        /// <summary>
        /// Tests controller response when fetching all of the existing orders.
        /// </summary>
        [Fact]
        public async void AdminController_FetchingAllOrders_OkObjectResultWithAllExistingOrders()
        {
            // Arrange
            var existingOrders = _fixture.Orders;
            foreach (var order in existingOrders)
            {
                order.Sneakers = _fixture.GetSneakersByOrderId(order.Id)
                    .ToDictionary(os => os.SneakerId, os => os.SneakerQty);
            }

            // Act
            var actionResult = await _fixture.Controller.GetAllOrdersAsync(new PaginationFilter());

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(existingOrders,
                    opts => opts.Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 100))
                        .WhenTypeIs<DateTime>());
        }

        /// <summary>
        /// Tests controller response when updating a user by invalid ID.
        /// </summary>
        [Fact]
        public async void AdminController_UpdatingGeneralUserInfoByID_NotFoundResult()
        {
            // Arrange
            var nonExistingId = 1000;
            var updateUserRequest = new UpdateUserRequest
            {
                Username = "kostyabek321",
                Email = "kostyabek@test.com",
                FirstName = "Named",
                LastName = "Surnamed",
                Patronymic = "Patronymicked",
                Age = 20,
                GenderId = 2
            };

            // Act
            var actionResult = await _fixture.Controller.UpdateUserGeneralInfoAsync(nonExistingId, updateUserRequest);

            // Assert
            actionResult.Should().BeOfType<NotFoundResult>();
        }

        /// <summary>
        /// Tests controller response when updating a sneaker by its valid ID.
        /// </summary>
        [Fact]
        public async void AdminController_UpdatingGeneralUserByID_OkObjectResultWithUpdatedUser()
        {
            // Arrange
            var existingId = _fixture.Users.ToList()[0].Id;
            var updateUserRequest = new UpdateUserRequest
            {
                Username = "kostyabek321",
                Email = "kostyabek@test.com",
                FirstName = "Named",
                LastName = "Surnamed",
                Patronymic = "Patronymicked",
                Age = 20,
                GenderId = 2
            };

            var userWithUpdateData = _fixture.Mapper.Map<User>(updateUserRequest);
            userWithUpdateData.Id = existingId;
            ((List<User>)_fixture.Users)[existingId - 1] = userWithUpdateData;

            // Act
            var actionResult = await _fixture.Controller.UpdateUserGeneralInfoAsync(existingId, updateUserRequest);

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(new List<User> { userWithUpdateData });
        }

        /// <summary>
        /// Tests controller response when deleting a user by invalid ID.
        /// </summary>
        [Fact]
        public async void UsersController_DeletingUserByID_NotFoundResult()
        {
            // Arrange
            var nonExistingId = 1000;

            ((List<User>)_fixture.Users).Remove(_fixture.Users.SingleOrDefault(s => s.Id == nonExistingId));

            // Act
            var actionResult = await _fixture.Controller.DeleteUserAsync(nonExistingId);

            // Assert
            actionResult.Should().BeOfType<NotFoundResult>();
            _fixture.Users.Count().Should().Be(3);
        }

        /// <summary>
        /// Tests controller response when deleting a user by valid ID.
        /// </summary>
        [Fact]
        public async void UsersController_DeletingUserByID_NoContentResult()
        {
            // Arrange
            var existingId = _fixture.Users.ToList()[0].Id;

            ((List<User>)_fixture.Users).Remove(_fixture.Users.SingleOrDefault(s => s.Id == existingId));

            // Act
            var actionResult = await _fixture.Controller.DeleteUserAsync(existingId);

            // Assert
            actionResult.Should().BeOfType<NoContentResult>();
            _fixture.Users.Where(s => s.Id == existingId).Should().BeEmpty();
        }
    }
}