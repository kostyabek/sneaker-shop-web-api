using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCamp_WEB_API.Core.Entities;

namespace BaseCamp_WEB_API.Core.RepositoryAbstractions
{
    /// <summary>
    /// Contains extra method signatures to be implemented by OrderSneakerRepository.
    /// </summary>
    public interface IOrderSneakerRepository : IGenericRepository<OrderSneaker, int>
    {
        /// <summary>
        /// Fetches all the sneakers related to an order with the given ID.
        /// </summary>
        /// <param name="orderId">ID of an order.</param>
        /// <returns></returns>
        Task<IEnumerable<OrderSneaker>> GetSneakersByOrderIdAsync(int orderId);

        /// <summary>
        /// Deletes a record with the given sneaker ID.
        /// </summary>
        /// <param name="sneakerInfo">Info about a sneaker in the order.</param>
        /// <returns></returns>
        /// <returns></returns>
        Task<int> DeleteBySneakerInfoAsync(OrderSneaker sneakerInfo);
    }
}