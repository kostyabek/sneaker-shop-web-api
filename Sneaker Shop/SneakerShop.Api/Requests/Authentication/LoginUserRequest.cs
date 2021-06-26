namespace BaseCamp_Web_API.Api.Requests.Authentication
{
    /// <summary>
    /// Contains user log-in credentials.
    /// </summary>
    public class LoginUserRequest
    {
        /// <summary>
        /// Gets or sets provided username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets provided password.
        /// </summary>
        public string Password { get; set; }
    }
}