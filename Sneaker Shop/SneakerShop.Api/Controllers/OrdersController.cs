using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BaseCamp_Web_API.Api.Requests;
using BaseCamp_Web_API.Api.Responses;
using BaseCamp_WEB_API.Core;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.Entities.Authorization.Attributes;
using BaseCamp_WEB_API.Core.Enums;
using BaseCamp_WEB_API.Core.Filters;
using BaseCamp_WEB_API.Core.RepositoryAbstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace BaseCamp_Web_API.Api.Controllers
{
    /// <summary>
    /// Controller for order entities.
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/v1.0/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderSneakerRepository _orderSneakerRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersController"/> class.
        /// </summary>
        /// <param name="orderRepository">For request processing to "orders" table.</param>
        /// <param name="orderSneakerRepository">For request processing to "orders_sneakers" table.</param>
        /// <param name="mapper">For entities mapping.</param>
        public OrdersController(IOrderRepository orderRepository,
            IOrderSneakerRepository orderSneakerRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _orderSneakerRepository = orderSneakerRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates new orders.
        /// </summary>
        /// <param name="orderRequest">"Order"-object to be created.</param>
        /// <returns>
        /// Created <see cref="Order"/> in <see cref="OkObjectResult"/>
        /// or status code 500 if database error encountered.
        /// </returns>
        [ClaimRequirement(ResourceTypes.Orders, ActionTypes.Create)]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateOrderRequest orderRequest)
        {
            orderRequest.DateTimeStamp = DateTime.Now;
            var order = _mapper.Map<Order>(orderRequest);
            order.StatusId = OrderStatuses.Accepted;
            Order orderResponse;

            try
            {
                order.Id = await _orderRepository.CreateAsync(order);

                foreach (var (sneakerId, sneakerQty) in order.Sneakers)
                {
                    var orderSneaker = new OrderSneaker
                        {
                            Id = order.Id,
                            SneakerId = sneakerId,
                            SneakerQty = sneakerQty
                        };
                    await _orderSneakerRepository.CreateAsync(orderSneaker);
                }

                orderResponse = (await _orderRepository.GetByIdAsync(order.Id)).FirstOrDefault();
                if (orderResponse != null)
                {
                    var sneakersFromOrder = (await _orderSneakerRepository.GetSneakersByOrderIdAsync(order.Id))
                        .ToDictionary(os => os.SneakerId, os => os.SneakerQty);
                    orderResponse.Sneakers = sneakersFromOrder;
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500);
            }

            return Ok(orderResponse);
        }

        /// <summary>
        /// Fetches all the orders.
        /// </summary>
        /// <param name="paginationFilter">Contains offset and limit for a database query.</param>
        /// <returns>Collection of <see cref="Order"/> from database in <see cref="OkObjectResult"/>.</returns>
        [ClaimRequirement(ResourceTypes.Orders, ActionTypes.Read)]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationFilter paginationFilter)
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
        /// Fetches orders with the given ID.
        /// </summary>
        /// <param name="id">ID of the record.</param>
        /// <returns>
        /// Collection with an <see cref="Order"/> in <see cref="OkObjectResult"/>.
        /// If no record found, then <see cref="NotFoundResult"/>.
        /// Status code 500 returned if database error encountered.
        /// </returns>
        [ClaimRequirement(ResourceTypes.Orders, ActionTypes.Read)]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            IEnumerable<OrderResponse> orderResponses = new List<OrderResponse>();
            try
            {
                orderResponses = _mapper
                    .Map<IEnumerable<OrderResponse>>(await _orderRepository.GetByIdAsync(id));

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

        /// <summary>
        /// Updates an order with the given ID.
        /// </summary>
        /// <param name="id">ID of the order to update.</param>
        /// <param name="orderRequest">Data to process.</param>
        /// <returns>
        /// Updated <see cref="Order"/> in <see cref="OkObjectResult"/>.
        /// Status code 500 returned if exception is encountered.
        /// </returns>
        [ClaimRequirement(ResourceTypes.Orders, ActionTypes.Update)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] UpdateOrderRequest orderRequest)
        {
            var order = _mapper.Map<Order>(orderRequest);
            order.Id = id;
            order.StatusId = (await _orderRepository.GetOrderStatusByIdAsync(id)).FirstOrDefault();

            Order response = null;

            try
            {
                var numOfRowsUpdated = await _orderRepository.UpdateAsync(id, order);
                if (numOfRowsUpdated == 0)
                {
                    return NotFound();
                }

                await _orderSneakerRepository.DeleteAsync(id);

                foreach (var (sneakerId, sneakerQty) in order.Sneakers)
                {
                    await _orderSneakerRepository.CreateAsync(new OrderSneaker
                    {
                        Id = id,
                        SneakerId = sneakerId,
                        SneakerQty = sneakerQty
                    });
                }

                response = (await _orderRepository.GetByIdAsync(id)).FirstOrDefault();
                var newOrderSneakers = (await _orderSneakerRepository.GetSneakersByOrderIdAsync(id))
                    .ToDictionary(os => os.SneakerId, os => os.SneakerQty);
                if (response != null)
                {
                    response.Sneakers = new Dictionary<int, int>();
                    foreach (var (sneakerId, sneakerQty) in newOrderSneakers)
                    {
                        response.Sneakers.Add(sneakerId, sneakerQty);
                    }
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500);
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500);
            }

            return Ok(response);
        }

        /// <summary>
        /// Deletes orders with the given ID.
        /// </summary>
        /// <param name="id">ID of the orders to delete.</param>
        /// <returns>
        /// <see cref="NoContentResult"/>.
        /// If no record found, then <see cref="NotFoundResult"/>.
        /// Status code 500 returned if database error encountered.
        /// </returns>
        [ClaimRequirement(ResourceTypes.Orders, ActionTypes.Delete)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            try
            {
                var numOfRowsDeleted = await _orderRepository.DeleteAsync(id);
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