namespace BaseCamp_Web_API.Api.Requests.Authentication
{
    /// <summary>
    /// Represents token requests.
    /// </summary>
    public class RefreshTokenRequest
    {
        /// <summary>
        /// Gets or sets current token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets current refresh token.
        /// </summary>
        public string RefreshToken { get; set; }
    }
}