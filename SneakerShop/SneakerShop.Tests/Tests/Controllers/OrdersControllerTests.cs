using System;
using System.Collections.Generic;
using System.Linq;
using BaseCamp_Web_API.Api.Requests;
using BaseCamp_Web_API.Api.Requests.Orders;
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
    /// Contains unit tests for OrdersController.
    /// </summary>
    public class OrdersControllerTests : IClassFixture<OrdersControllerTestsFixture>
    {
        private readonly OrdersControllerTestsFixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersControllerTests"/> class.
        /// </summary>
        /// <param name="fixture">Contains resources for tests.</param>
        public OrdersControllerTests(OrdersControllerTestsFixture fixture)
        {
            _fixture = fixture;
            _fixture.ResetOrderCollection();
            _fixture.ResetOrderSneakerCollection();
        }

        /// <summary>
        /// Tests controller response when creating a new order.
        /// </summary>
        [Fact]
        public async void OrdersController_CreatingNewOrder_OkObjectResultWithCreatedOrder()
        {
            // Arrange
            var createOrderRequest = new CreateOrderRequest
            {
                 UserId = 4,
                 DeliveryAddress = "marvelous delivery address",
                 DateTimeStamp = DateTime.UtcNow,
                 Sneakers = new Dictionary<int, int> { { 1, 1 } }
            };

            // Act
            var order = _fixture.Mapper.Map<Order>(createOrderRequest);
            order.Id = 4;
            ((List<Order>)_fixture.Orders).Add(order);
            var actionResult = await _fixture.Controller.CreateAsync(createOrderRequest);

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(order, opts => opts
                    .Excluding(o => o.Id)
                    .Excluding(o => o.StatusId));

            _fixture.OrderSneakers.Where(os => os.Id == order.Id)
                .ToDictionary(os => os.SneakerId, os => os.SneakerQty)
                .Should().BeEquivalentTo(order.Sneakers);
        }

        /// <summary>
        /// Tests controller response when fetching a order by invalid ID.
        /// </summary>
        /// <param name="id">ID of an order to get.</param>
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1000)]
        public async void OrdersController_FetchingOrderByID_NotFoundResult(int id)
        {
            // Arrange

            // Act
            var actionResult = await _fixture.Controller.GetByIdForUserAsync(id);

            // Assert
            actionResult.Should().BeOfType<NotFoundObjectResult>();
        }

        /// <summary>
        /// Tests controller response when fetching a order by valid ID.
        /// </summary>
        [Fact]
        public async void OrdersController_FetchingOrderByID_OkObjectResultWithIEnumerableWithOrder()
        {
            // Arrange
            var existingOrderId = 3;
            var existingOrder = _fixture.Orders.FirstOrDefault(o => o.Id == existingOrderId);
            existingOrder.Sneakers = _fixture.GetSneakersByOrderId(existingOrderId)
                .ToDictionary(os => os.SneakerId, os => os.SneakerQty);

            // Act
            var actionResult = await _fixture.Controller.GetByIdForUserAsync(existingOrderId);

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(existingOrder);
        }

        /// <summary>
        /// Tests controller response when updating an order by invalid ID.
        /// </summary>
        [Fact]
        public async void OrdersController_UpdatingOrderByID_NotFoundResult()
        {
            // Arrange
            var nonExistingId = 1000;
            var updateOrderRequest = new UpdateOrderRequest
            {
                UserId = 5,
                DeliveryAddress = "some new delivery address",
                DateTimeStamp = DateTime.UtcNow,
                Sneakers = new Dictionary<int, int> { { 1, 10 } },
                StatusId = OrderStatuses.Completed
            };

            // Act
            var actionResult = await _fixture.Controller.UpdateAsync(nonExistingId, updateOrderRequest);

            // Assert
            actionResult.Should().BeOfType<NotFoundResult>();
        }

        /// <summary>
        /// Tests controller response when updating an order by valid ID.
        /// </summary>
        [Fact]
        public async void OrdersController_UpdatingOrderByID_OkObjectResultWithIEnumerableWithOrder()
        {
            // Arrange
            var existingId = 3;
            var updateOrderRequest = new UpdateOrderRequest
            {
                UserId = 5,
                DeliveryAddress = "some new delivery address",
                DateTimeStamp = DateTime.UtcNow,
                Sneakers = new Dictionary<int, int> { { 1, 10 } },
                StatusId = OrderStatuses.Completed
            };
            var updatedOrder = _fixture.Mapper.Map<Order>(updateOrderRequest);
            updatedOrder.Id = existingId;
            var orderToUpdateIndex = ((List<Order>)_fixture.Orders).FindIndex(o => o.Id == existingId);
            ((List<Order>)_fixture.Orders)[orderToUpdateIndex] = updatedOrder;

            // Act
            var actionResult = await _fixture.Controller.UpdateAsync(existingId, updateOrderRequest);

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>().Which.Value.Should().BeEquivalentTo(updatedOrder,
                opts => opts
                    .Excluding(o => o.StatusId)
                    .Excluding(o => o.TotalMoney)
                    .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, 100))
                    .WhenTypeIs<DateTime>());

            _fixture.OrderSneakers.Where(os => os.Id == existingId)
                .ToDictionary(os => os.SneakerId, os => os.SneakerQty)
                .Should().BeEquivalentTo(updatedOrder.Sneakers);
        }

        /// <summary>
        /// Tests controller response when deleting an order by invalid ID.
        /// </summary>
        [Fact]
        public async void OrdersController_DeletingOrderByID_NotFoundResult()
        {
            // Arrange
            var nonExistingId = 1000;

            ((List<Order>)_fixture.Orders).Remove(_fixture.Orders.SingleOrDefault(s => s.Id == nonExistingId));

            // Act
            var actionResult = await _fixture.Controller.DeleteAsync(nonExistingId);

            // Assert
            actionResult.Should().BeOfType<NotFoundResult>();
            _fixture.Orders.Count().Should().Be(3);
        }

        /// <summary>
        /// Tests controller response when deleting an order by valid ID.
        /// </summary>
        [Fact]
        public async void OrdersController_DeletingOrderByID_NoContentResult()
        {
            // Arrange
            var existingId = _fixture.Orders.ToList()[0].Id;

            // Act
            var actionResult = await _fixture.Controller.DeleteAsync(existingId);

            ((List<Order>)_fixture.Orders).Remove(_fixture.Orders.SingleOrDefault(s => s.Id == existingId));

            // Assert
            actionResult.Should().BeOfType<NoContentResult>();
            _fixture.Orders.Where(s => s.Id == existingId).Should().BeEmpty();
        }
    }
}