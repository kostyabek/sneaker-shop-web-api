using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BaseCamp_Web_API.Api.Requests.Users;
using BaseCamp_Web_API.Api.Responses;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.Filters;
using BaseCamp_WEB_API.Core.RepositoryAbstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace BaseCamp_Web_API.Api.Controllers
{
    /// <summary>
    /// Controller for administrator-specific actions.
    /// </summary>
    [ApiController]
    [Route("api/v1.0/admin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderSneakerRepository _orderSneakerRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminController"/> class.
        /// </summary>
        /// <param name="userRepository">For request processing about users.</param>
        /// <param name="orderRepository">For request processing about orders.</param>
        /// <param name="mapper">For entities mapping.</param>
        /// <param name="orderSneakerRepository">For request processing about sneakers from orders.</param>
        public AdminController(IUserRepository userRepository,
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
        /// Fetches all the orders.
        /// </summary>
        /// <param name="paginationFilter">Contains offset and limit for a database query.</param>
        /// <returns>Collection of <see cref="Order"/> from database in <see cref="OkObjectResult"/>.</returns>
        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrdersAsync([FromQuery] PaginationFilter paginationFilter)
        {
            var orders = _mapper.Map<IEnumerable<OrderResponse>>(await _orderRepository.GetAllAsync(paginationFilter));

            foreach (var order in orders)
            {
                order.Sneakers = (await _orderSneakerRepository.GetSneakersByOrderIdAsync(order.Id))
                    .ToDictionary(os => os.SneakerId, os => os.SneakerQty);
            }

            return Ok(orders);
        }

        /// <summary>
        /// Updates a user with the given ID.
        /// </summary>
        /// <param name="id">ID of a user to update.</param>
        /// <param name="request">"Data to be processed.</param>
        /// <returns>
        /// Updated <see cref="User"/> in <see cref="OkObjectResult"/>.
        /// If no record found, then <see cref="NotFoundResult"/>.
        /// Status code 500 returned if exception is encountered.
        /// </returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUserGeneralInfoAsync([FromRoute] int id, [FromBody] UpdateUserRequest request)
        {
            var user = _mapper.Map<User>(request);
            user.Id = id;

            try
            {
                var numOfRowsUpdated = await _userRepository.UpdateAsync(id, user);
                if (numOfRowsUpdated == 0)
                {
                    return NotFound();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500);
            }

            return Ok(await _userRepository.GetByIdAsync(id));
        }

        /// <summary>
        /// Deletes a user with the given ID.
        /// </summary>
        /// <param name="id">ID of a user to delete.</param>
        /// <returns>
        /// <see cref="NoContentResult"/>.
        /// If no record found, then <see cref="NotFoundResult"/>.
        /// Status code 500 returned if database error encountered.
        /// </returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] int id)
        {
            try
            {
                var numOfRowsDeleted = await _userRepository.DeleteAsync(id);
                if (numOfRowsDeleted == 0)
                {
                    return NotFound();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500);
            }

            return NoContent();
        }
    }
}