using BaseCamp_WEB_API.Core.Enums;

namespace BaseCamp_Web_API.Api.Requests.Orders
{
    /// <summary>
    /// Entity for "update" requests.
    /// </summary>
    public class UpdateOrderRequest : CreateOrderRequest
    {
        public OrderStatuses StatusId { get; set; }
    }
}