using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BaseCamp_Web_API.Api.Responses;
using BaseCamp_WEB_API.Core;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.Entities.Authorization.Attributes;
using BaseCamp_WEB_API.Core.Filters;
using BaseCamp_WEB_API.Core.RepositoryAbstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace BaseCamp_Web_API.Api.Controllers
{
    /// <summary>
    /// Controller for user entities.
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "CanReadUsers")]
    [ApiController]
    [Route("api/v1.0/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderSneakerRepository _orderSneakerRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// Constructor for this controller.
        /// </summary>
        /// <param name="userRepository">For request processing about users.</param>
        /// <param name="orderRepository">For request processing about orders.</param>
        /// <param name="mapper">For entities mapping.</param>
        /// <param name="orderSneakerRepository">For request processing about sneakers from orders.</param>
        public UsersController(IUserRepository userRepository,
            IOrderRepository orderRepository,
            IOrderSneakerRepository orderSneakerRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _orderSneakerRepository = orderSneakerRepository;
            _orderRepository = orderRepository;
        }

        /// <summary>
        /// Fetches all the users.
        /// </summary>
        /// <param name="paginationFilter">Contains offset and limit for a database query.</param>
        /// <returns>Collection of <see cref="User"/> from database in <see cref="OkObjectResult"/>.</returns>
        [ClaimRequirement(ResourceTypes.Users, ActionTypes.Read)]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationFilter paginationFilter)
        {
            return Ok(_mapper.
                Map<IEnumerable<UserResponse>>(await _userRepository
                    .GetAllAsync(paginationFilter)));
        }

        /// <summary>
        /// Fetches a user with the given ID.
        /// </summary>
        /// <param name="id">ID of a user.</param>
        /// <returns>
        /// Collection with a <see cref="User"/> in <see cref="OkObjectResult"/>.
        /// If no record found, then <see cref="NotFoundResult"/>.
        /// Status code 500 returned if database error encountered.
        /// </returns>
        [ClaimRequirement(ResourceTypes.Users, ActionTypes.Read)]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            IEnumerable<UserResponse> response = new List<UserResponse>();
            try
            {
                response = _mapper.Map<IEnumerable<UserResponse>>(await _userRepository.GetByIdAsync(id));
                if (!response.Any())
                {
                    return NotFound();
                }

                return Ok(response);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Fetches orders of the specific user.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <returns>
        /// Collection with an <see cref="Order"/> in <see cref="OkObjectResult"/>.
        /// If no record found, then <see cref="NotFoundResult"/>.
        /// Status code 500 returned if database error encountered.
        /// </returns>
        [ClaimRequirement(ResourceTypes.Orders, ActionTypes.Read)]
        [HttpGet("{userId:int}/orders")]
        public async Task<IActionResult> GetOrdersByUserIdAsync([FromRoute] int userId)
        {
            IEnumerable<OrderResponse> orderResponses = new List<OrderResponse>();
            try
            {
                orderResponses = _mapper
                    .Map<IEnumerable<OrderResponse>>(await _orderRepository.GetByUserIdAsync(userId));

                if (!orderResponses.Any())
                {
                    return NotFound();
                }

                foreach (var response in orderResponses)
                {
                    response.Sneakers = (await _orderSneakerRepository.GetSneakersByOrderIdAsync(response.Id))
                        .ToDictionary(os => os.SneakerId, os => os.SneakerQty);
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500);
            }

            return Ok(orderResponses);
        }
    }
}