using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BaseCamp_Web_API.Api;
using BaseCamp_Web_API.Api.Controllers;
using BaseCamp_Web_API.Api.Requests;
using BaseCamp_Web_API.Api.Requests.Sneakers;
using BaseCamp_Web_API.Api.Responses;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.Filters;
using BaseCamp_WEB_API.Data.Repositories;
using BaseCamp_Web_API.Tests.Tests.Controllers;
using Moq;
using SqlKata.Execution;

namespace BaseCamp_Web_API.Tests.Fixtures.Controllers
{
    /// <summary>
    /// Contains needed resources and additional functionality for tests in <see cref="SneakersControllerTests"/>.
    /// </summary>
    public class SneakersControllerTestsFixture
    {
        /// <summary>
        /// Controller object for endpoint testing.
        /// </summary>
        public readonly SneakersController Controller;

        /// <summary>
        /// Mapper.
        /// </summary>
        public readonly IMapper Mapper;

        /// <summary>
        /// Collection of sneakers representing a database table.
        /// </summary>
        public IEnumerable<Sneaker> Sneakers;

        /// <summary>
        /// Initializes a new instance of the <see cref="SneakersControllerTestsFixture"/> class.
        /// </summary>
        public SneakersControllerTestsFixture()
        {
            Sneakers = GetAllSneakers();

            var mapperConfig = new MapperConfiguration(opts =>
            {
                opts.AddProfile<MappingProfile>();
            });
            Mapper = mapperConfig.CreateMapper();

            var queryFactoryMock = new Mock<QueryFactory>();
            var repositoryMock = new Mock<SneakerRepository>(queryFactoryMock.Object);
            repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Sneaker>()))
                .ReturnsAsync(6);
            repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<PaginationFilter>()))
                .ReturnsAsync(Sneakers);
            repositoryMock.Setup(r => r.GetByIdAsync(It.Is<int>(i => i > 0 && i < 6)))
                .ReturnsAsync((int id) => GetSneakerById(id));
            repositoryMock.Setup(r => r.UpdateAsync(It.Is<int>(i => i > 0 && i < 6), It.IsAny<Sneaker>()))
                .ReturnsAsync(1);
            repositoryMock.Setup(r => r.DeleteAsync(It.Is<int>(i => i > 0 && i < 6)))
                .ReturnsAsync(1);

            Controller = new SneakersController(Mapper, repositoryMock.Object);
        }

        /// <summary>
        /// Returns collection of sneakers for testing.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Sneaker> GetAllSneakers()
        {
            return new List<Sneaker>
            {
                new () { Id = 1, Price = 149.99M, ModelName = "997H", VendorId = 3, GenderId = 1, SeasonId = 1 },
                new () { Id = 2, Price = 129.79M, ModelName = "AIR FORCE 1", VendorId = 2, GenderId = 1, SeasonId = 3 },
                new () { Id = 3, Price = 139.99M, ModelName = "GEL-LYTE G-TX", VendorId = 1, GenderId = 1, SeasonId = 4 },
                new () { Id = 4, Price = 119.59M, ModelName = "DUNK LOW", VendorId = 4, GenderId = 2, SeasonId = 1 },
                new () { Id = 5, Price = 109.49M, ModelName = "OLD SKOOL TNT", VendorId = 5, GenderId = 2, SeasonId = 2 },
            };
        }

        /// <summary>
        /// Returns a sneaker by its ID for testing.
        /// </summary>
        /// <param name="id">ID of a sneaker to get.</param>
        /// <returns></returns>
        public IEnumerable<Sneaker> GetSneakerById(int id)
        {
            return Sneakers.Where(s => s.Id == id);
        }

        /// <summary>
        /// Resets Sneaker collection to its initial state.
        /// </summary>
        public void ResetSneakerCollection()
        {
            Sneakers = GetAllSneakers();
        }
    }
}