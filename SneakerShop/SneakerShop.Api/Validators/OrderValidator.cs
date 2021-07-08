using BaseCamp_Web_API.Api.Requests;
using BaseCamp_Web_API.Api.Requests.Orders;
using BaseCamp_Web_API.Api.Validators.ErrorMessages;
using FluentValidation;

namespace BaseCamp_Web_API.Api.Validators
{
    /// <summary>
    /// Validator for data received from order-related requests.
    /// </summary>
    public class OrderValidator : AbstractValidator<CreateOrderRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderValidator"/> class.
        /// </summary>
        public OrderValidator()
        {
            RuleFor(o => o.UserId)
                .GreaterThanOrEqualTo(1).WithMessage(OrderValidatorMessages.UserIdIsInvalid);

            RuleFor(o => o.Sneakers)
                .NotEmpty().WithMessage(OrderValidatorMessages.SneakerCollectionIsEmpty);

            RuleFor(o => o.DeliveryAddress)
                .NotEmpty().WithMessage(OrderValidatorMessages.DeliveryAddressIsEmpty);
        }
    }
}