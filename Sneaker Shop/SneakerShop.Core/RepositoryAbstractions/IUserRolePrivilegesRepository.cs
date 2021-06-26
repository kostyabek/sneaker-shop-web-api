using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCamp_WEB_API.Core.Entities.Authorization;

namespace BaseCamp_WEB_API.Core.RepositoryAbstractions
{
    /// <summary>
    /// Contains extra method signatures to be implemented by UserRoleRepository.
    /// </summary>
    public interface IUserRolePrivilegesRepository : IGenericRepository<UserRoleWithPrivileges, int>
    {
        /// <summary>
        /// Fetches privileges .
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <param name="resourceType">Name of the resource.</param>
        /// <returns></returns>
        Task<IEnumerable<UserRoleWithPrivileges>> GetPrivilegesForResourceByUserId(int userId, string resourceType);
    }
}