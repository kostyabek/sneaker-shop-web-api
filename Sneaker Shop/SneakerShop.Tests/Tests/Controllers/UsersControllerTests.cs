using System.Collections.Generic;
using System.Linq;
using BaseCamp_Web_API.Api.Requests;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.Enums;
using BaseCamp_WEB_API.Core.Filters;
using BaseCamp_Web_API.Tests.Fixtures.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BaseCamp_Web_API.Tests.Tests.Controllers
{
    /// <summary>
    /// Contains unit tests for UsersController.
    /// </summary>
    public class UsersControllerTests : IClassFixture<UsersControllerTestsFixture>
    {
        private readonly UsersControllerTestsFixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersControllerTests"/> class.
        /// </summary>
        /// <param name="fixture">Contains resources for tests.</param>
        public UsersControllerTests(UsersControllerTestsFixture fixture)
        {
            _fixture = fixture;
            _fixture.ResetUsersCollection();
            _fixture.ResetOrderCollection();
            _fixture.ResetOrderSneakerCollection();
        }

        /// <summary>
        /// Tests controller response when fetching all of the existing users.
        /// </summary>
        [Fact]
        public async void UsersController_FetchingAllUsers_OkObjectResultWithAllExistingUsers()
        {
            // Arrange
            var existingUsers = _fixture.Users;

            // Act
            var actionResult = await _fixture.Controller.GetAllAsync(new PaginationFilter());

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(existingUsers);
        }

        /// <summary>
        /// Tests controller response when fetching a user by invalid ID.
        /// </summary>
        /// <param name="id">ID of a user to fetch.</param>
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1000)]
        public async void UsersController_FetchingUserByID_OkObjectResultWithEmptyIEnumerable(int id)
        {
            // Arrange
            var existingUser = _fixture.GetUserById(id);

            // Act
            var actionResult = await _fixture.Controller.GetByIdAsync(id);

            // Assert
            actionResult.Should().BeOfType<NotFoundResult>();
        }

        /// <summary>
        /// Tests controller response when fetching a user by valid ID.
        /// </summary>
        [Fact]
        public async void UsersController_FetchingUserByID_OkObjectResultWithIEnumerableWithUser()
        {
            // Arrange
            var existingId = _fixture.GetAllUsers().ToList()[0].Id;
            var existingUser = _fixture.GetUserById(existingId);

            // Act
            var actionResult = await _fixture.Controller.GetByIdAsync(existingId);

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(existingUser);
        }

        /// <summary>
        /// Tests controller response when fetching specific user's orders by invalid user ID.
        /// </summary>
        /// <param name="userId">ID of a user to get.</param>
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(1000)]
        public async void UsersController_FetchingOrdersByUserID_NotFoundResult(int userId)
        {
            // Arrange

            // Act
            var actionResult = await _fixture.Controller.GetOrdersByUserIdAsync(userId);

            // Assert
            actionResult.Should().BeOfType<NotFoundResult>();
        }

        /// <summary>
        /// Tests controller response when fetching specific user's orders by valid user ID.
        /// </summary>
        [Fact]
        public async void OrdersController_FetchingOrdersByUserID_OkObjectResultWithIEnumerableWithOrders()
        {
            // Arrange
            var existingUserId = 5;
            var orders = _fixture.GetOrdersByUserId(existingUserId);
            foreach (var order in orders)
            {
                order.Sneakers = _fixture.GetSneakersByOrderId(order.Id)
                    .ToDictionary(os => os.SneakerId, os => os.SneakerQty);
            }

            // Act
            var actionResult = await _fixture.Controller.GetOrdersByUserIdAsync(existingUserId);

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(orders);
        }

        /// <summary>
        /// Tests controller response when updating a user by invalid ID.
        /// </summary>
        [Fact]
        public async void UsersController_UpdatingUserByID_NotFoundResult()
        {
            // Arrange
            var nonExistingId = 1000;
            var updateUserRequest = new UpdateUserRequest
            {
                Username = "kostyabek321",
                Email = "kostyabek@test.com",
                RoleId = UserRoles.Customer,
                FirstName = "Named",
                LastName = "Surnamed",
                Patronymic = "Patronymicked",
                Age = 20,
                GenderId = 2
            };

            // Act
            var actionResult = await _fixture.Controller.UpdateGeneralInfoAsync(nonExistingId, updateUserRequest);

            // Assert
            actionResult.Should().BeOfType<NotFoundResult>();
        }

        /// <summary>
        /// Tests controller response when updating a sneaker by its valid ID.
        /// </summary>
        [Fact]
        public async void UsersController_UpdatingUserByID_OkObjectResultWithUpdatedUser()
        {
            // Arrange
            var existingId = _fixture.Users.ToList()[0].Id;
            var updateUserRequest = new UpdateUserRequest
            {
                Username = "kostyabek321",
                Email = "kostyabek@test.com",
                RoleId = UserRoles.Customer,
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
            var actionResult = await _fixture.Controller.UpdateGeneralInfoAsync(existingId, updateUserRequest);

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
            var actionResult = await _fixture.Controller.DeleteAsync(nonExistingId);

            // Assert
            actionResult.Should().BeOfType<NotFoundResult>();
            _fixture.Users.Count().Should().Be(3);
        }

        /// <summary>
        /// Tests controller response when deleting a sneaker by its ID.
        /// </summary>
        [Fact]
        public async void UsersController_DeletingUserByID_NoContentResult()
        {
            // Arrange
            var existingId = _fixture.Users.ToList()[0].Id;

            ((List<User>)_fixture.Users).Remove(_fixture.Users.SingleOrDefault(s => s.Id == existingId));

            // Act
            var actionResult = await _fixture.Controller.DeleteAsync(existingId);

            // Assert
            actionResult.Should().BeOfType<NoContentResult>();
            _fixture.Users.Where(s => s.Id == existingId).Should().BeEmpty();
        }
    }
}