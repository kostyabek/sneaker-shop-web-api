using System;

namespace BaseCamp_Web_API.Api.Configuration
{
    /// <summary>
    /// Contains refresh-token lifetime.
    /// </summary>
    public class RefreshTokenConfig
    {
        /// <summary>
        /// Gets or sets token lifetime.
        /// </summary>
        public TimeSpan TokenLifetime { get; set; }
    }
}