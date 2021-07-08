using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.RepositoryAbstractions;
using SqlKata.Execution;

namespace BaseCamp_WEB_API.Data.Repositories
{
    /// <summary>
    /// Works with the "orders_sneakers" table.
    /// </summary>
    public class OrderSneakerRepository : GenericRepository<OrderSneakerPair, int>, IOrderSneakerRepository
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
        public override Task<int> UpdateAsync(int id, OrderSneakerPair entity)
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
        public Task<int> DeleteBySneakerInfoAsync(OrderSneakerPair sneakerPairInfo)
        {
            return queryFactory
                .Query(NodeTableName)
                .Where(new
                {
                    id = sneakerPairInfo.Id,
                    sneakerId = sneakerPairInfo.SneakerId
                })
                .DeleteAsync();
        }

        /// <inheritdoc/>
        public virtual Task<IEnumerable<OrderSneakerPair>> GetSneakersByOrderIdAsync(int orderId)
        {
            return queryFactory
                .Query(NodeTableName)
                .Where(nameof(BaseModel<object>.Id), orderId)
                .GetAsync<OrderSneakerPair>();
        }
    }
}