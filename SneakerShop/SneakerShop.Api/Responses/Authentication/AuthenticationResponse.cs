using System.Collections.Generic;

namespace BaseCamp_Web_API.Api.Responses.Authentication
{
    /// <summary>
    /// Gives information about authentication result.
    /// </summary>
    public class AuthenticationResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether indicates whether the registration is successful or not.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets current token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets current refresh token.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets collection of occurred errors.
        /// </summary>
        public IEnumerable<string> Errors { get; set; }
    }
}