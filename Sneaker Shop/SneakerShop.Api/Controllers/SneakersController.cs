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
using BaseCamp_WEB_API.Core.Filters;
using BaseCamp_WEB_API.Core.RepositoryAbstractions;
using BaseCamp_WEB_API.Data.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace BaseCamp_Web_API.Api.Controllers
{
    /// <summary>
    /// Controller for sneaker entities.
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/v1.0/sneakers")]
    public class SneakersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISneakerRepository _sneakerRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SneakersController"/> class.
        /// </summary>
        /// /// <param name="mapper">For entities mapping.</param>
        /// <param name="sneakerRepository">For request processing.</param>
        public SneakersController(IMapper mapper, ISneakerRepository sneakerRepository)
        {
            _sneakerRepository = sneakerRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates new records.
        /// </summary>
        /// <param name="sneakerRequest">Data to be processed.</param>
        /// <returns>
        /// Created <see cref="Sneaker"/> in <see cref="OkObjectResult"/>
        /// or status code 500 if database error encountered.
        /// </returns>
        [ClaimRequirement(ResourceTypes.Sneakers, ActionTypes.Create)]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateSneakerRequest sneakerRequest)
        {
            var sneaker = _mapper.Map<Sneaker>(sneakerRequest);
            SneakerRepository.PutFieldsToUppercase(sneaker);
            try
            {
                sneaker.Id = await _sneakerRepository.CreateAsync(sneaker);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500);
            }

            return Ok(sneaker);
        }

        /// <summary>
        /// Fetches all the sneakers.
        /// </summary>
        /// <param name="paginationFilter">Contains offset and limit for a database query.</param>
        /// <returns>Collection of <see cref="Sneaker"/> from database in <see cref="OkObjectResult"/>.</returns>
        [ClaimRequirement(ResourceTypes.Sneakers, ActionTypes.Read)]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationFilter paginationFilter)
        {
            return Ok(_mapper.Map<IEnumerable<SneakerResponse>>(await _sneakerRepository.GetAllAsync(paginationFilter)));
        }

        /// <summary>
        /// Fetches records with the given ID.
        /// </summary>
        /// <param name="id">ID of the sneaker.</param>
        /// <returns>
        /// Collection with a <see cref="Sneaker"/> in <see cref="OkObjectResult"/>.
        /// If no record found, then <see cref="NotFoundResult"/>.
        /// Status code 500 returned if database error encountered.
        /// </returns>
        [ClaimRequirement(ResourceTypes.Sneakers, ActionTypes.Read)]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            IEnumerable<SneakerResponse> response = new List<SneakerResponse>();
            try
            {
                response = _mapper
                    .Map<IEnumerable<SneakerResponse>>(await _sneakerRepository.GetByIdAsync(id));

                if (!response.Any())
                {
                    return NotFound();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500);
            }

            return Ok(response);
        }

        /// <summary>
        /// Updates a sneaker with the given ID.
        /// </summary>
        /// <param name="id">ID of a sneaker to update.</param>
        /// <param name="sneakerRequest">Data to be processed.</param>
        /// <returns>
        /// Updated <see cref="Sneaker"/> in <see cref="OkObjectResult"/>.
        /// If no record found, then <see cref="NotFoundResult"/>.
        /// Status code 500 returned if exception is encountered.
        /// </returns>
        [ClaimRequirement(ResourceTypes.Sneakers, ActionTypes.Update)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id, [FromBody] UpdateSneakerRequest sneakerRequest)
        {
            try
            {
                var sneaker = _mapper.Map<Sneaker>(sneakerRequest);
                sneaker.Id = id;
                SneakerRepository.PutFieldsToUppercase(sneaker);

                var numOfRowsUpdated = await _sneakerRepository.UpdateAsync(id, sneaker);
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

            return Ok(await _sneakerRepository.GetByIdAsync(id));
        }

        /// <summary>
        /// Deletes a sneaker with the given ID.
        /// </summary>
        /// <param name="id">ID of a sneaker to delete.</param>
        /// <returns>
        /// <see cref="NoContentResult"/>.
        /// If no record found, then <see cref="NotFoundResult"/>.
        /// Status code 500 returned if database error encountered.
        /// </returns>
        [ClaimRequirement(ResourceTypes.Sneakers, ActionTypes.Delete)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            try
            {
                var numOfRowsDeleted = await _sneakerRepository.DeleteAsync(id);
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