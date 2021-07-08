using System;

namespace BaseCamp_Web_API.Api.Configuration
{
    /// <summary>
    /// Contains JWT secret key.
    /// </summary>
    public class JwtConfig
    {
        /// <summary>
        /// Gets or sets JWT secret key.
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Gets or sets JWT token lifetime.
        /// </summary>
        public TimeSpan TokenLifetime { get; set; }
    }
}