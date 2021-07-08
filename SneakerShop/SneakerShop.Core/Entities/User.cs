using System.ComponentModel.DataAnnotations;
using BaseCamp_WEB_API.Core.Enums;

namespace BaseCamp_WEB_API.Core.Entities
{
    /// <summary>
    /// Represents "User" entity.
    /// </summary>
    public class User : BaseModel<int>
    {
        /// <summary>
        /// Gets or sets user's username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets user's password.
        /// </summary>
        public string HashedPassword { get; set; }

        /// <summary>
        /// Gets or sets user's e-mail.
        /// </summary>
        [EmailAddress]
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

        /*public int UserId { get; set; }

        public int PrivilegeId { get; set; }

        public bool Create { get; set; }

        public bool Get { get; set; }

        public bool Update { get; set; }

        public bool Delete { get; set; }*/
    }
}