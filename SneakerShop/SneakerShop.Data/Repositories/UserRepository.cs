using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.RepositoryAbstractions;
using SqlKata.Execution;

namespace BaseCamp_WEB_API.Data.Repositories
{
    /// <summary>
    /// Works with the 'users' table.
    /// </summary>
    public class UserRepository : GenericRepository<User, int>, IUserRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="queryFactory">SQLKata object for query creation.</param>
        public UserRepository(QueryFactory queryFactory) : base(queryFactory)
        {
            NodeTableName = "users";
        }

        /// <summary>
        /// Puts all text fields except for username and password to uppercase.
        /// </summary>
        /// <param name="user">User to process.</param>
        public static void PutFieldsToUppercase(User user)
        {
            var userProperties = user
                .GetType()
                .GetProperties()
                .Where(p => p.PropertyType.Name == "String");

            foreach (var propertyInfo in userProperties)
            {
                if (propertyInfo.Name is nameof(User.HashedPassword) or nameof(User.Username))
                {
                    continue;
                }

                var newValue = propertyInfo
                    .GetValue(user)?
                    .ToString()?
                    .ToUpper();

                propertyInfo.SetValue(user, newValue);
            }
        }

        /// <summary>
        /// Puts username to lowercase.
        /// </summary>
        /// <param name="user">User to process.</param>
        public static void PutUsernameToLowercase(User user)
        {
            user.Username = user.Username.ToLower();
        }

        /// <inheritdoc/>
        public virtual Task<IEnumerable<User>> GetUserByUsernameAsync(string username)
        {
            username = username.ToLower();
            return queryFactory
                .Query(NodeTableName)
                .Where(nameof(User.Username), username)
                .GetAsync<User>();
        }

        /// <inheritdoc/>
        public virtual Task<IEnumerable<User>> GetUserByEmailAsync(string email)
        {
            return queryFactory
                    .Query(NodeTableName)
                    .Where(nameof(User.Email), email)
                    .GetAsync<User>();
        }
    }
}