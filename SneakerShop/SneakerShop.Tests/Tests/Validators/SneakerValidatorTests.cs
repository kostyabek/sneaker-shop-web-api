using BaseCamp_Web_API.Api.Requests;
using BaseCamp_Web_API.Api.Requests.Sneakers;
using BaseCamp_Web_API.Api.Validators.ErrorMessages;
using BaseCamp_Web_API.Tests.Fixtures.Validators;
using FluentAssertions;
using Xunit;

namespace BaseCamp_Web_API.Tests.Tests.Validators
{
    /// <summary>
    /// Contains unit tests for SneakerValidator.
    /// </summary>
    public class SneakerValidatorTests : IClassFixture<SneakerValidatorTestsFixture>
    {
        private readonly SneakerValidatorTestsFixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="SneakerValidatorTests"/> class.
        /// </summary>
        /// <param name="fixture">Contains resources for tests.</param>
        public SneakerValidatorTests(SneakerValidatorTestsFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Tests validator response when model name of a new sneaker is empty.
        /// </summary>
        [Fact]
        public void SneakerValidator_CreatingNewSneakerAndSneakerModelNameIsEmpty_ErrorAboutModelNameIsEmpty()
        {
            // Arrange
            var createSneakerRequest = new CreateSneakerRequest
            {
                ModelName = string.Empty,
                VendorId = 1,
                SeasonId = 1,
                GenderId = 1,
                Price = 199.99M
            };

            // Act
            var validationResult = _fixture.Validator.Validate(createSneakerRequest);

            // Assert
            validationResult.Errors.Count.Should().Be(1);
            validationResult.Errors[0].ErrorMessage.Should().Be(SneakerValidatorMessages.ModelNameIsEmpty);
        }

        /// <summary>
        /// Tests validator response when vendor ID of a new sneaker is not found.
        /// </summary>
        /// <param name="vendorId">Test value for vendor ID.</param>
        [Theory]
        [InlineData(-1000)]
        [InlineData(0)]
        [InlineData(1000)]
        public void SneakerValidator_CreatingNewSneakerAndVendorIdNotFound_ErrorAboutVendorIdNotFound(int vendorId)
        {
            // Arrange
            var createSneakerRequest = new CreateSneakerRequest
            {
                ModelName = "Some Model",
                VendorId = vendorId,
                SeasonId = 1,
                GenderId = 1,
                Price = 199.99M
            };

            // Act
            var validationResult = _fixture.Validator.Validate(createSneakerRequest);

            // Assert
            validationResult.Errors.Count.Should().Be(1);
            validationResult.Errors[0].ErrorMessage.Should().Be(SneakerValidatorMessages.VendorIdNotFound);
        }

        /// <summary>
        /// Tests validator response when season ID of a new sneaker is not found.
        /// </summary>
        /// <param name="seasonId">Test value for season ID.</param>
        [Theory]
        [InlineData(-1000)]
        [InlineData(0)]
        [InlineData(1000)]
        public void SneakerValidator_CreatingNewSneakerAndSeasonIdNotFound_ErrorAboutSeasonIdNotFound(int seasonId)
        {
            // Arrange
            var createSneakerRequest = new CreateSneakerRequest
            {
                ModelName = "Some Model",
                VendorId = 1,
                SeasonId = seasonId,
                GenderId = 1,
                Price = 199.99M
            };

            // Act
            var validationResult = _fixture.Validator.Validate(createSneakerRequest);

            // Assert
            validationResult.Errors.Count.Should().Be(1);
            validationResult.Errors[0].ErrorMessage.Should().Be(SneakerValidatorMessages.SeasonIdNotFound);
        }

        /// <summary>
        /// Tests validator response when gender ID of a new sneaker is not found.
        /// </summary>
        /// <param name="genderId">Test value for gender ID.</param>
        [Theory]
        [InlineData(-1000)]
        [InlineData(0)]
        [InlineData(1000)]
        public void SneakerValidator_CreatingNewSneakerAndGenderIdNotFound_ErrorAboutGenderIdNotFound(int genderId)
        {
            // Arrange
            var createSneakerRequest = new CreateSneakerRequest
            {
                ModelName = "Some Model",
                VendorId = 1,
                SeasonId = 1,
                GenderId = genderId,
                Price = 199.99M
            };

            // Act
            var validationResult = _fixture.Validator.Validate(createSneakerRequest);

            // Assert
            validationResult.Errors.Count.Should().Be(1);
            validationResult.Errors[0].ErrorMessage.Should().Be(SneakerValidatorMessages.GenderIdNotFound);
        }

        /// <summary>
        /// Tests validator response when price of a new sneaker is invalid.
        /// </summary>
        /// <param name="price">Test value for price.</param>
        [Theory]
        [InlineData(-1000.05)]
        [InlineData(0)]
        public void SneakerValidator_CreatingNewSneakerAndItsPriceIsInvalid_ErrorAboutPriceNotValid(decimal price)
        {
            // Arrange
            var createSneakerRequest = new CreateSneakerRequest
            {
                ModelName = "Some Model",
                VendorId = 1,
                SeasonId = 1,
                GenderId = 1,
                Price = price
            };

            // Act
            var validationResult = _fixture.Validator.Validate(createSneakerRequest);

            // Assert
            validationResult.Errors.Count.Should().Be(1);
            validationResult.Errors[0].ErrorMessage.Should().Be(SneakerValidatorMessages.PriceIsZeroOrLess);
        }
    }
}