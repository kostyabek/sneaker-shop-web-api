namespace BaseCamp_Web_API.Api.Requests.Sneakers
{
    /// <summary>
    /// Entity for working with "create" requests.
    /// </summary>
    public class CreateSneakerRequest
    {
        /// <summary>
        /// Gets or sets sneaker model name.
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// Gets or sets sneaker vendor.
        /// </summary>
        public int VendorId { get; set; }

        /// <summary>
        /// Gets or sets sneaker season of wearing.
        /// </summary>
        public int SeasonId { get; set; }

        /// <summary>
        /// Gets or sets sneaker target gender.
        /// </summary>
        public int GenderId { get; set; }

        /// <summary>
        /// Gets or sets sneaker price.
        /// </summary>
        public decimal Price { get; set; }
    }
}