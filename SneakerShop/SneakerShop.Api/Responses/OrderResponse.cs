using System;
using System.Collections.Generic;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.Enums;

namespace BaseCamp_Web_API.Api.Responses
{
    /// <summary>
    /// Entity for "get" requests.
    /// </summary>
    public class OrderResponse : BaseModel<int>
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
        public decimal TotalMoney { get; set; }

        /// <summary>
        /// Gets or sets delivery address.
        /// </summary>
        public string DeliveryAddress { get; set; }

        /// <summary>
        /// Gets or sets order status ID.
        /// </summary>
        public OrderStatuses StatusId { get; set; }

        /// <summary>
        /// Gets or sets ordered sneakers and their quantity.
        /// </summary>
        public Dictionary<int, int> Sneakers { get; set; }
    }
}