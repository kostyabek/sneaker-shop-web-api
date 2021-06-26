using BaseCamp_WEB_API.Core.Enums;

namespace BaseCamp_Web_API.Api.Requests.Authentication
{
    /// <summary>
    /// Entity for working with "create" requests.
    /// </summary>
    public class RegisterUserRequest
    {
        /// <summary>
        /// Gets or sets provided username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets provided password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets user's e-mail.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets user's role.
        /// </summary>
        public UserRoles RoleId { get; set; }

        /// <summary>
        /// Gets or sets user's first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets user's last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets user's patronymic.
        /// </summary>
        public string Patronymic { get; set; }

        /// <summary>
        /// Gets or sets user's age.
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// Gets or sets user's sex.
        /// </summary>
        public int GenderId { get; set; }
    }
}