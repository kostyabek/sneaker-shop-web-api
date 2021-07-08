using System;

namespace BaseCamp_Web_API.Api.Configuration
{
    public class RefreshTokenConfig
    {
        public TimeSpan TokenLifetime { get; set; }
    }
}