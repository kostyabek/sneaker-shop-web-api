using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using BaseCamp_Web_API.Api;
using BaseCamp_Web_API.Api.Controllers;
using BaseCamp_Web_API.Api.Requests;
using BaseCamp_Web_API.Api.Requests.Orders;
using BaseCamp_Web_API.Api.Responses;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.Enums;
using BaseCamp_WEB_API.Core.Filters;
using BaseCamp_WEB_API.Data.Repositories;
using BaseCamp_Web_API.Tests.Tests.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SqlKata.Execution;

namespace BaseCamp_Web_API.Tests.Fixtures.Controllers
{
    /// <summary>
    /// Contains needed resources and additional functionality for tests in <see cref="OrdersControllerTests"/>.
    /// </summary>
    public class OrdersControllerTestsFixture
    {
        /// <summary>
        /// Controller instance for testing.
        /// </summary>
        public readonly OrdersController Controller;

        /// <summary>
        /// Mapper.
        /// </summary>
        public readonly IMapper Mapper;

        /// <summary>
        /// Collection of orders for testing.
        /// </summary>
        public IEnumerable<Order> Orders;

        /// <summary>
        /// Collection of sneakers from orders for testing.
        /// </summary>
        public IEnumerable<OrderSneakerPair> OrderSneakers;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersControllerTestsFixture"/> class.
        /// </summary>
        public OrdersControllerTestsFixture()
        {
            var mapperConfig = new MapperConfiguration(opts =>
            {
                opts.AddProfile<MappingProfile>();
            });
            Mapper = mapperConfig.CreateMapper();

            var queryFactoryMock = new Mock<QueryFactory>();
            var orderRepositoryMock = new Mock<OrderRepository>(queryFactoryMock.Object);
            orderRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Order>()))
                .ReturnsAsync(4);
            orderRepositoryMock.Setup(r => r.GetByIdAsync(It.Is<int>(i => i > 0 && i < 5)))
                .ReturnsAsync((int id) => GetOrderById(id));
            orderRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<PaginationFilter>()))
                .ReturnsAsync(GetAllOrders);
            orderRepositoryMock.Setup(r => r.GetByUserIdAsync(It.Is<int>(i => i > 3 && i < 6)))
                .ReturnsAsync((int userId) => GetOrdersByUserId(userId));
            orderRepositoryMock.Setup(r => r.UpdateAsync(It.Is<int>(i => i > 0 && i < 4), It.IsAny<Order>()))
                .ReturnsAsync(1);
            orderRepositoryMock.Setup(r => r.DeleteAsync(It.Is<int>(i => i > 0 && i < 4)))
                .ReturnsAsync(1);
            orderRepositoryMock.Setup(r => r.GetOrderStatusByIdAsync(It.Is<int>(i => i > 0)))
                .ReturnsAsync(new List<OrderStatuses> { OrderStatuses.Accepted });

            var orderSneakerRepositoryMock = new Mock<OrderSneakerRepository>(queryFactoryMock.Object);
            orderSneakerRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<OrderSneakerPair>()))
                .ReturnsAsync(4);
            orderSneakerRepositoryMock.Setup(r => r.GetSneakersByOrderIdAsync(It.Is<int>(i => i > 0 && i < 4)))
                .ReturnsAsync((int id) => GetSneakersByOrderId(id));
            orderSneakerRepositoryMock.Setup(r => r.DeleteAsync(It.Is<int>(i => i > 0 && i < 4)))
                .ReturnsAsync(1);

            Controller = new OrdersController(orderRepositoryMock.Object, orderSneakerRepositoryMock.Object, Mapper);
            Controller.ControllerContext = new ControllerContext();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Sid, "5") }));
            Controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };
        }

        /// <summary>
        /// Returns collection of orders for testing.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Order> GetAllOrders()
        {
            return new List<Order>
            {
                new () { Id = 1, UserId = 5, DeliveryAddress = "some delivery address", DateTimeStamp = DateTime.UtcNow, Sneakers = new Dictionary<int, int>(), StatusId = OrderStatuses.Accepted, TotalMoney = 200.04M },
                new () { Id = 2, UserId = 4, DeliveryAddress = "another delivery address", DateTimeStamp = DateTime.UtcNow, Sneakers = new Dictionary<int, int>(), StatusId = OrderStatuses.Reviewed, TotalMoney = 300.04M },
                new () { Id = 3, UserId = 5, DeliveryAddress = "doomed delivery address", DateTimeStamp = DateTime.UtcNow, Sneakers = new Dictionary<int, int>(), StatusId = OrderStatuses.OnTheWay, TotalMoney = 400.04M },
            };
        }

        /// <summary>
        /// Return collection of sneaker info from orders for testing.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OrderSneakerPair> GetAllOrderSneakers()
        {
            return new List<OrderSneakerPair>
            {
                new () { Id = 1, SneakerId = 1, SneakerQty = 2 },
                new () { Id = 1, SneakerId = 2, SneakerQty = 1 },
                new () { Id = 2, SneakerId = 1, SneakerQty = 3 },
                new () { Id = 2, SneakerId = 3, SneakerQty = 2 },
                new () { Id = 3, SneakerId = 4, SneakerQty = 1 }
            };
        }

        /// <summary>
        /// Returns an order by its ID for testing.
        /// </summary>
        /// <param name="id">ID of an order to get.</param>
        /// <returns></returns>
        public IEnumerable<Order> GetOrderById(int id)
        {
            return Orders.Where(o => o.Id == id);
        }

        /// <summary>
        /// Returns an sneaker info from order by its ID for testing.
        /// </summary>
        /// <param name="id">ID of an order to get.</param>
        /// <returns></returns>
        public IEnumerable<OrderSneakerPair> GetSneakersByOrderId(int id)
        {
            return OrderSneakers.Where(o => o.Id == id);
        }

        /// <summary>
        /// Returns orders of a specific user bu his/her ID..
        /// </summary>
        /// <param name="userId">ID of an order to get.</param>
        /// <returns></returns>
        public IEnumerable<Order> GetOrdersByUserId(int userId)
        {
            return Orders.Where(o => o.UserId == userId);
        }

        /// <summary>
        /// Resets Sneaker collection to its initial state.
        /// </summary>
        public void ResetOrderCollection()
        {
            Orders = GetAllOrders();
        }

        /// <summary>
        /// Resets OrderSneaker collection to its initial state.
        /// </summary>
        public void ResetOrderSneakerCollection()
        {
            OrderSneakers = GetAllOrderSneakers();
        }
    }
}