using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.Enums;
using BaseCamp_WEB_API.Core.RepositoryAbstractions;
using SqlKata.Execution;

namespace BaseCamp_WEB_API.Data.Repository
{
    /// <summary>
    /// Works with "orders" and "orders_sneakers" tables.
    /// </summary>
    public class OrderRepository : GenericRepository<Order, int>, IOrderRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderRepository"/> class.
        /// </summary>
        /// <param name="queryFactory">SQLKta object for query creation.</param>
        public OrderRepository(QueryFactory queryFactory) : base(queryFactory)
        {
            NodeTableName = "orders";
        }

        /// <inheritdoc/>
        public virtual Task<IEnumerable<Order>> GetByUserIdAsync(int userId)
        {
            return queryFactory.Query(NodeTableName)
                .Where(nameof(userId), userId)
                .GetAsync<Order>();
        }

        /// <inheritdoc/>
        public virtual Task<IEnumerable<OrderStatuses>> GetOrderStatusByIdAsync(int id)
        {
            return queryFactory.Query(NodeTableName)
                .Select(nameof(Order.StatusId))
                .Where(nameof(Order.Id), id)
                .GetAsync<OrderStatuses>();
        }
    }
}