namespace BaseCamp_Web_API.Api.Validators.ErrorMessages
{
    /// <summary>
    /// Contains error messages for SneakerValidator.
    /// </summary>
    public static class SneakerValidatorMessages
    {
        /// <summary>
        /// Error message for vendor ID.
        /// </summary>
        public const string VendorIdNotFound = "There is no vendor with such ID!";

        /// <summary>
        /// Error message for season ID.
        /// </summary>
        public const string SeasonIdNotFound = "There is no season with such ID!";

        /// <summary>
        /// Error message for model name.
        /// </summary>
        public const string ModelNameIsEmpty = "Model name must not be empty.";

        /// <summary>
        /// Error message for gender ID.
        /// </summary>
        public const string GenderIdNotFound = "There is no gender with such ID!";

        /// <summary>
        /// Error message for price.
        /// </summary>
        public const string PriceIsZeroOrLess = "Price must be a positive number!";
    }
}