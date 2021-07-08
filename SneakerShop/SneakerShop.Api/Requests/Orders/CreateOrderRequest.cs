using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BaseCamp_Web_API.Api.Requests.Orders
{
    /// <summary>
    /// Entity for working with "create" requests.
    /// </summary>
    public class CreateOrderRequest
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
        /// Gets or sets delivery address.
        /// </summary>
        public string DeliveryAddress { get; set; }

        /// <summary>
        /// Gets or sets ordered sneakers and their quantity.
        /// </summary>
        public Dictionary<int, int> Sneakers { get; set; }
    }
}