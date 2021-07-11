using AutoMapper;
using BaseCamp_Web_API.Api.Requests.Authentication;
using BaseCamp_Web_API.Api.Requests.Orders;
using BaseCamp_Web_API.Api.Requests.Sneakers;
using BaseCamp_Web_API.Api.Requests.Users;
using BaseCamp_Web_API.Api.Responses;
using BaseCamp_WEB_API.Core.Entities;

namespace BaseCamp_Web_API.Api
{
    /// <summary>
    /// Maps entities by their correspondent entities.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProfile"/> class.
        /// </summary>
        public MappingProfile()
        {
            CreateMap<CreateOrderRequest, Order>();
            CreateMap<UpdateOrderRequest, Order>();
            CreateMap<Order, OrderResponse>();

            CreateMap<RegisterUserRequest, User>();
            CreateMap<UpdateUserRequest, User>();
            CreateMap<User, UserResponse>();

            CreateMap<CreateSneakerRequest, Sneaker>();
            CreateMap<UpdateSneakerRequest, Sneaker>();
            CreateMap<Sneaker, SneakerResponse>();
        }
    }
}