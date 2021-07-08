using System;
using System.Collections.Generic;
using BaseCamp_Web_API.Api.Requests;
using BaseCamp_Web_API.Api.Requests.Orders;
using BaseCamp_Web_API.Api.Validators;
using BaseCamp_Web_API.Api.Validators.ErrorMessages;
using FluentAssertions;
using Xunit;

namespace BaseCamp_Web_API.Tests.Tests.Validators
{
    /// <summary>
    /// Contains unit tests for OrderValidator.
    /// </summary>
    public class OrderValidatorTests
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderValidatorTests"/> class.
        /// </summary>
        public OrderValidatorTests()
        {
            Validator = new OrderValidator();
        }

        /// <summary>
        /// Gets or sets validator object.
        /// </summary>
        private OrderValidator Validator { get; set; }

        /// <summary>
        /// Tests validator response when user ID in the order is not found.
        /// </summary>
        /// <param name="userId">Test value for vendor ID.</param>
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void OrderValidator_CreatingNewOrderAndUserIdIsLesserThanOne_ErrorAboutIdValueRange(int userId)
        {
            // Arrange
            var createOrderRequest = new CreateOrderRequest
            {
                DateTimeStamp = DateTime.UtcNow,
                DeliveryAddress = "some address",
                UserId = userId,
                Sneakers = new Dictionary<int, int>()
            };
            createOrderRequest.Sneakers.Add(1, 1);

            // Act
            var validationResult = Validator.Validate(createOrderRequest);

            // Assert
            validationResult.Errors.Count.Should().Be(1);
            validationResult.Errors[0].ErrorMessage.Should().Be(OrderValidatorMessages.UserIdIsInvalid);
        }

        /// <summary>
        /// Tests validator response when user ID in the order is not found.
        /// </summary>
        [Fact]
        public void OrderValidator_CreatingNewOrderAndUserIdIsLesserThanOne_NoErrors()
        {
            // Arrange
            var createOrderRequest = new CreateOrderRequest
            {
                DateTimeStamp = DateTime.UtcNow,
                DeliveryAddress = "some address",
                UserId = 1,
                Sneakers = new Dictionary<int, int>()
            };
            createOrderRequest.Sneakers.Add(1, 1);

            // Act
            var validationResult = Validator.Validate(createOrderRequest);

            // Assert
            validationResult.Errors.Should().BeEmpty();
        }

        /// <summary>
        /// Tests validator response when user ID in the order is not found.
        /// </summary>
        [Fact]
        public void OrderValidator_CreatingNewOrderAndSneakerCollectionIsEmpty_ErrorAboutEmptySneakerCollection()
        {
            // Arrange
            var createOrderRequest = new CreateOrderRequest
            {
                DateTimeStamp = DateTime.UtcNow,
                DeliveryAddress = "some address",
                UserId = 5,
                Sneakers = new Dictionary<int, int>()
            };

            // Act
            var validationResult = Validator.Validate(createOrderRequest);

            // Assert
            validationResult.Errors.Count.Should().Be(1);
            validationResult.Errors[0].ErrorMessage.Should().Be(OrderValidatorMessages.SneakerCollectionIsEmpty);
        }

        /// <summary>
        /// Tests validator response when delivery address in the order is empty.
        /// </summary>
        [Fact]
        public void OrderValidator_CreatingNewOrderAndDeliveryAddressIsEmpty_ErrorAboutEmptyDeliveryAddress()
        {
            // Arrange
            var createOrderRequest = new CreateOrderRequest
            {
                DateTimeStamp = DateTime.UtcNow,
                DeliveryAddress = string.Empty,
                UserId = 5,
                Sneakers = new Dictionary<int, int>()
            };
            createOrderRequest.Sneakers.Add(1, 1);

            // Act
            var validationResult = Validator.Validate(createOrderRequest);

            // Assert
            validationResult.Errors.Count.Should().Be(1);
            validationResult.Errors[0].ErrorMessage.Should().Be(OrderValidatorMessages.DeliveryAddressIsEmpty);
        }
    }
}