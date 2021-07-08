using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.Entities.Authorization;
using BaseCamp_WEB_API.Core.RepositoryAbstractions;
using SqlKata.Execution;

namespace BaseCamp_WEB_API.Data.Repositories
{
    /// <summary>
    /// Works with the "user_role_privileges" table.
    /// </summary>
    public class UserRoleWithPrivilegesRepository : GenericRepository<UserRoleWithPrivileges, int>, IUserRoleWithPrivilegesRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRoleWithPrivilegesRepository"/> class.
        /// </summary>
        /// <param name="queryFactory">SQLKata object for query creation.</param>
        public UserRoleWithPrivilegesRepository(QueryFactory queryFactory) : base(queryFactory)
        {
            NodeTableName = "user_role_privileges";
        }

        /// <inheritdoc/>
        public Task<IEnumerable<UserRoleWithPrivileges>> GetPrivilegesForResourceByUserId(int userId, string resourceType)
        {
            return queryFactory.Query("users")
                .Join(NodeTableName, $"users.{nameof(User.RoleId)}", $"{NodeTableName}.{nameof(User.RoleId)}")
                .Where($"users.{nameof(User.Id)}", userId)
                .Where($"{NodeTableName}.{nameof(UserRoleWithPrivileges.Resource)}", resourceType)
                .GetAsync<UserRoleWithPrivileges>();
        }
    }
}