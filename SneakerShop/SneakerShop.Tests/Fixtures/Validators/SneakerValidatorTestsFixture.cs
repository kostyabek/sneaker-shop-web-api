using System.Collections.Generic;
using BaseCamp_Web_API.Api.Validators;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.Filters;
using BaseCamp_WEB_API.Core.RepositoryAbstractions;
using BaseCamp_WEB_API.Data.Repositories;
using BaseCamp_Web_API.Tests.Tests.Validators;
using Moq;

namespace BaseCamp_Web_API.Tests.Fixtures.Validators
{
    /// <summary>
    /// Contains needed resources and additional functionality for tests in <see cref="SneakerValidatorTests"/>>.
    /// </summary>
    public class SneakerValidatorTestsFixture
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SneakerValidatorTestsFixture"/> class.
        /// </summary>
        public SneakerValidatorTestsFixture()
        {
            VendorRepo = new Mock<IVendorRepository>();
            VendorRepo.Setup(vr => vr.GetAllAsync(It.IsAny<PaginationFilter>()))
                .ReturnsAsync(GetVendors);
            SeasonRepo = new Mock<ISeasonRepository>();
            SeasonRepo.Setup(sr => sr.GetAllAsync(It.IsAny<PaginationFilter>()))
                .ReturnsAsync(GetSeasons);

            Validator = new SneakerValidator(VendorRepo.Object, SeasonRepo.Object);
        }

        /// <summary>
        /// Gets or sets mocked <see cref="VendorRepository"/>.
        /// </summary>
        public Mock<IVendorRepository> VendorRepo { get; set; }

        /// <summary>
        /// Gets or sets mocked <see cref="SeasonRepository"/>.
        /// </summary>
        public Mock<ISeasonRepository> SeasonRepo { get; set; }

        /// <summary>
        /// Gets or sets <see cref="SneakerValidator"/>.
        /// </summary>
        public SneakerValidator Validator { get; set; }

        /// <summary>
        /// Returns a collection of existing sneaker vendors.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Vendor> GetVendors()
        {
            return new List<Vendor>
            {
                new () { Id = 1, Name = "NIKE" },
                new () { Id = 2, Name = "ADIDAS" },
                new () { Id = 3, Name = "PUMA" },
            };
        }

        /// <summary>
        /// Returns a collection of existing seasons for sneakers.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Season> GetSeasons()
        {
            return new List<Season>
            {
                new () { Id = 1, Name = "SUMMER" },
                new () { Id = 2, Name = "SPRING SUMMER" },
                new () { Id = 3, Name = "AUTUMN SPRING" },
                new () { Id = 4, Name = "WINTER" }
            };
        }
    }
}