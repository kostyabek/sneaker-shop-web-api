namespace BaseCamp_Web_API.Api.Validators.ErrorMessages
{
    /// <summary>
    /// Contains error messages for OrderValidator.
    /// </summary>
    public static class OrderValidatorMessages
    {
        /// <summary>
        /// Error message for user ID.
        /// </summary>
        public const string UserIdIsInvalid = "User ID should be a positive number.";

        /// <summary>
        /// Error message for sneaker collection.
        /// </summary>
        public const string SneakerCollectionIsEmpty = "Sneakers dictionary should not be empty.";

        /// <summary>
        /// Error message for delivery address.
        /// </summary>
        public const string DeliveryAddressIsEmpty = "Delivery address should not be empty.";
    }
}