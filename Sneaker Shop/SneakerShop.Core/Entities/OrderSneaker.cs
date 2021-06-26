namespace BaseCamp_WEB_API.Core.Entities
{
    /// <summary>
    /// Represents a row of data from the "orders_sneakers" table.
    /// </summary>
    public class OrderSneaker : BaseModel<int>
    {
        /// <summary>
        /// Gets or sets sneaker ID from the order.
        /// </summary>
        public int SneakerId { get; set; }

        /// <summary>
        /// Gets or sets quantity of the sneakers ordered with the SneakerId.
        /// </summary>
        public int SneakerQty { get; set; }
    }
}