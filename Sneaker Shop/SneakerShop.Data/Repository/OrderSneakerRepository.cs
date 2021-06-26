using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.RepositoryAbstractions;
using SqlKata.Execution;

namespace BaseCamp_WEB_API.Data.Repository
{
    /// <summary>
    /// Works with the "orders_sneakers" table.
    /// </summary>
    public class OrderSneakerRepository : GenericRepository<OrderSneaker, int>, IOrderSneakerRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderSneakerRepository"/> class.
        /// </summary>
        /// <param name="queryFactory">SQLKta object for query creation.</param>
        public OrderSneakerRepository(QueryFactory queryFactory) : base(queryFactory)
        {
            NodeTableName = "orders_sneakers";
        }

        /// <inheritdoc/>
        public override Task<int> UpdateAsync(int id, OrderSneaker entity)
        {
            return queryFactory
                .Query(NodeTableName)
                .Where(new
                {
                    id,
                    sneakerId = entity.SneakerId
                })
                .UpdateAsync(entity);
        }

        /// <inheritdoc/>
        public Task<int> DeleteBySneakerInfoAsync(OrderSneaker sneakerInfo)
        {
            return queryFactory
                .Query(NodeTableName)
                .Where(new
                {
                    id = sneakerInfo.Id,
                    sneakerId = sneakerInfo.SneakerId
                })
                .DeleteAsync();
        }

        /// <inheritdoc/>
        public virtual Task<IEnumerable<OrderSneaker>> GetSneakersByOrderIdAsync(int orderId)
        {
            return queryFactory
                .Query(NodeTableName)
                .Where(nameof(BaseModel<object>.Id), orderId)
                .GetAsync<OrderSneaker>();
        }
    }
}