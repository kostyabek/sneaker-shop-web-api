using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.Enums;

namespace BaseCamp_WEB_API.Core.RepositoryAbstractions
{
    /// <summary>
    /// Contains extra method signatures to be implemented by OrderRepository.
    /// </summary>
    public interface IOrderRepository : IGenericRepository<Order, int>
    {
        /// <summary>
        /// Fetches orders by given user ID.
        /// </summary>
        /// <param name="userId">ID of the user.</param>
        /// <returns></returns>
        Task<IEnumerable<Order>> GetByUserIdAsync(int userId);

        /// <summary>
        /// Fetches order status by its ID.
        /// </summary>
        /// <param name="id">ID of the order.</param>
        /// <returns></returns>
        Task<IEnumerable<OrderStatuses>> GetOrderStatusByIdAsync(int id);
    }
}