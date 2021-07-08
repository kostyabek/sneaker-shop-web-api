using BaseCamp_WEB_API.Core.Entities;

namespace BaseCamp_Web_API.Api.Responses
{
    /// <summary>
    /// Entity for "get" requests.
    /// </summary>
    public class SneakerResponse : BaseModel<int>
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