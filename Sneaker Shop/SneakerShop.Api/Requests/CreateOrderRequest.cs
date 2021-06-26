using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BaseCamp_Web_API.Api.Requests
{
    /// <summary>
    /// Entity for working with "create" requests.
    /// </summary>
    public class CreateOrderRequest
    {
        /// <summary>
        /// Gets or sets order's user ID.
        /// </summary>
        [Required(ErrorMessage = "User ID field is required!")]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets date and time of an order.
        /// </summary>
        [Required(ErrorMessage = "Date and time field is required!")]
        public DateTime DateTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets delivery address.
        /// </summary>
        [Required(ErrorMessage = "Delivery address field is required!")]
        public string DeliveryAddress { get; set; }

        /// <summary>
        /// Gets or sets ordered sneakers and their quantity.
        /// </summary>
        [Required(ErrorMessage = "Sneakers field is required!")]
        public Dictionary<int, int> Sneakers { get; set; }
    }
}