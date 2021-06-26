using System;
using System.Collections.Generic;
using BaseCamp_WEB_API.Core.Enums;
using SqlKata;

namespace BaseCamp_WEB_API.Core.Entities
{
    /// <summary>
    /// Represents "Order"-entities.
    /// </summary>
    public class Order : BaseModel<int>
    {
        /// <summary>
        /// Gets or sets order's user ID.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets date and time of an order.
        /// </summary>
        public DateTime DateTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets total sum of money to pay for an order.
        /// </summary>
        [Ignore]
        public decimal TotalMoney { get; set; }

        /// <summary>
        /// Gets or sets ordered sneakers and their quantity.
        /// </summary>
        [Ignore]
        public Dictionary<int, int> Sneakers { get; set; }

        /// <summary>
        /// Gets or sets delivery address.
        /// </summary>
        public string DeliveryAddress { get; set; }

        /// <summary>
        /// Gets or sets order status ID.
        /// </summary>
        public OrderStatuses StatusId { get; set; }
    }
}