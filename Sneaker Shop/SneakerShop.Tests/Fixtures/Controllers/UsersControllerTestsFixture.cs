using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BaseCamp_Web_API.Api.Controllers;
using BaseCamp_Web_API.Api.Requests;
using BaseCamp_Web_API.Api.Requests.Authentication;
using BaseCamp_Web_API.Api.Responses;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.Enums;
using BaseCamp_WEB_API.Core.Filters;
using BaseCamp_WEB_API.Data.Repository;
using BaseCamp_Web_API.Tests.Tests.Controllers;
using Moq;
using SqlKata.Execution;

namespace BaseCamp_Web_API.Tests.Fixtures.Controllers
{
    /// <summary>
    /// Contains needed resources and additional functionality for tests in <see cref="UsersControllerTests"/>.
    /// </summary>
    public class UsersControllerTestsFixture
    {
        /// <summary>
        /// Controller for endpoint testing.
        /// </summary>
        public readonly UsersController Controller;

        /// <summary>
        /// Users collection representing a database table;
        /// </summary>
        public IEnumerable<User> Users;

        public IEnumerable<Order> Orders;

        public IEnumerable<OrderSneaker> OrderSneakers;

        /// <summary>
        /// Mapper.
        /// </summary>
        public readonly IMapper Mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersControllerTestsFixture"/> class.
        /// </summary>
        public UsersControllerTestsFixture()
        {
            Users = GetAllUsers();

            var mapperConfig = new MapperConfiguration(opts =>
            {
                opts.CreateMap<RegisterUserRequest, User>();
                opts.CreateMap<UpdateUserRequest, User>();
                opts.CreateMap<User, GetUserResponse>();
                opts.CreateMap<CreateOrderRequest, Order>();
                opts.CreateMap<UpdateOrderRequest, Order>();
                opts.CreateMap<Order, OrderResponse>();
            });
            Mapper = mapperConfig.CreateMapper();

            var queryFactoryMock = new Mock<QueryFactory>();
            var userRepositoryMock = new Mock<UserRepository>(queryFactoryMock.Object);
            userRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<PaginationFilter>()))
                .ReturnsAsync(Users);
            userRepositoryMock.Setup(r => r.GetByIdAsync(It.Is<int>(i => i > 0 && i < 4)))
                .ReturnsAsync((int id) => GetUserById(id));
            userRepositoryMock.Setup(r => r.UpdateAsync(It.Is<int>(i => i > 0 && i < 4), It.IsAny<User>()))
                .ReturnsAsync(1);
            userRepositoryMock.Setup(r => r.DeleteAsync(It.Is<int>(i => i > 0 && i < 4)))
                .ReturnsAsync(1);

            var orderRepositoryMock = new Mock<OrderRepository>(queryFactoryMock.Object);
            orderRepositoryMock.Setup(r => r.GetByUserIdAsync(It.Is<int>(i => i > 3 && i < 6)))
                .ReturnsAsync((int userId) => GetOrdersByUserId(userId));

            var orderSneakerRepositoryMock = new Mock<OrderSneakerRepository>(queryFactoryMock.Object);
            orderSneakerRepositoryMock.Setup(r => r.GetSneakersByOrderIdAsync(It.Is<int>(i => i > 0 && i < 4)))
                .ReturnsAsync((int id) => GetSneakersByOrderId(id));

            Controller = new UsersController(userRepositoryMock.Object,
                orderRepositoryMock.Object,
                orderSneakerRepositoryMock.Object,
                Mapper);
        }

        /// <summary>
        /// Returns a user by ID for testing.
        /// </summary>
        /// <param name="id">ID of a sneaker to get.</param>
        /// <returns></returns>
        public IEnumerable<User> GetUserById(int id)
        {
            return Users.Where(u => u.Id == id);
        }

        /// <summary>
        /// Returns collection of users for testing.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<User> GetAllUsers()
        {
            return new List<User>
            {
                new ()
                {
                    Id = 1,
                    Username = "kostyabek",
                    HashedPassword =
                        "AQAAAAEAACcQAAAAEEUed/H8W883JN7hcdNSMW1LB2bmJHD3Jc13IKdlrk8OjtSKkWjHEOdO45i1SaZcbQ==",
                    Email = "kostyabek@test.com",
                    RoleId = UserRoles.Customer,
                    FirstName = "Name",
                    LastName = "Surname",
                    Patronymic = "Patronymic",
                    Age = 25,
                    GenderId = 1
                },
                new ()
                {
                    Id = 2,
                    Username = "mortimer",
                    HashedPassword =
                        "AQAAAAEAACcQAAAAEEUed/H8W883JN7hcdNSMW1LB2bmJHD3Jc13IKdlrk8OjtSKkWjHEOdO45i1SaZcbQ==",
                    Email = "mortimer@test.com",
                    RoleId = UserRoles.Customer,
                    FirstName = "Name",
                    LastName = "Surname",
                    Patronymic = "Patronymic",
                    Age = 25,
                    GenderId = 1
                },
                new ()
                {
                    Id = 3,
                    Username = "muffin",
                    HashedPassword =
                        "AQAAAAEAACcQAAAAEEUed/H8W883JN7hcdNSMW1LB2bmJHD3Jc13IKdlrk8OjtSKkWjHEOdO45i1SaZcbQ==",
                    Email = "muffin@test.com",
                    RoleId = UserRoles.Customer,
                    FirstName = "Name",
                    LastName = "Surname",
                    Patronymic = "Patronymic",
                    Age = 25,
                    GenderId = 1
                }
            };
        }

        /// <summary>
        /// Resets User collection to its initial state.
        /// </summary>
        public void ResetUsersCollection()
        {
            Users = GetAllUsers();
        }

        /// <summary>
        /// Returns an sneaker info from order by its ID for testing.
        /// </summary>
        /// <param name="id">ID of an order to get.</param>
        /// <returns></returns>
        public IEnumerable<OrderSneaker> GetSneakersByOrderId(int id)
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
        /// Return collection of sneaker info from orders for testing.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OrderSneaker> GetAllOrderSneakers()
        {
            return new List<OrderSneaker>
            {
                new () { Id = 1, SneakerId = 1, SneakerQty = 2 },
                new () { Id = 1, SneakerId = 2, SneakerQty = 1 },
                new () { Id = 2, SneakerId = 1, SneakerQty = 3 },
                new () { Id = 2, SneakerId = 3, SneakerQty = 2 },
                new () { Id = 3, SneakerId = 4, SneakerQty = 1 }
            };
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