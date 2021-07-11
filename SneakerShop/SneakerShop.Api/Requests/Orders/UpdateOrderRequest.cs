using BaseCamp_WEB_API.Core.Enums;

namespace BaseCamp_Web_API.Api.Requests.Orders
{
    /// <summary>
    /// Entity for "update" requests.
    /// </summary>
    public class UpdateOrderRequest : CreateOrderRequest
    {
        /// <summary>
        /// Gets or sets order status ID.
        /// </summary>
        public OrderStatuses StatusId { get; set; }
    }
}