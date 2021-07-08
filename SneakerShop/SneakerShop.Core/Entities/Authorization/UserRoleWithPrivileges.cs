using BaseCamp_WEB_API.Core.Enums;

namespace BaseCamp_WEB_API.Core.Entities.Authorization
{
    /// <summary>
    /// Contains data related with user roles and their privileges.
    /// </summary>
    public class UserRoleWithPrivileges : BaseModel<int>
    {
        /// <summary>
        /// Gets or sets role ID.
        /// </summary>
        public UserRoles RoleId { get; set; }

        /// <summary>
        /// Gets or sets name of the resource.
        /// </summary>
        public string Resource { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a role has right to create.
        /// </summary>
        public bool Create { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a role has right to get.
        /// </summary>
        public bool Read { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a role has right to update.
        /// </summary>
        public bool Update { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a role has right to delete.
        /// </summary>
        public bool Delete { get; set; }
    }
}