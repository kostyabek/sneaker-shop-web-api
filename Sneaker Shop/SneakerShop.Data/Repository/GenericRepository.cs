using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.Filters;
using BaseCamp_WEB_API.Core.RepositoryAbstractions;
using SqlKata.Execution;

namespace BaseCamp_WEB_API.Data.Repository
{
    /// <summary>
    /// Contains general implementation of CRUD-operations for repositories.
    /// </summary>
    /// <typeparam name="T1">Entity type for repository to work with.</typeparam>
    /// <typeparam name="T2">Data type of the ID for entities.</typeparam>
    public class GenericRepository<T1, T2> : IGenericRepository<T1, T2> where T1 : BaseModel<T2>
    {

        /// <summary>
        /// SQLKata object for query creation.
        /// </summary>
        protected readonly QueryFactory queryFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository{T1, T2}"/> class.
        /// </summary>
        /// <param name="queryFactory">SQLKta object for query creation.</param>
        protected GenericRepository(QueryFactory queryFactory)
        {
            this.queryFactory = queryFactory;
        }

        /// <summary>
        /// Gets or sets target database table name for this repository.
        /// </summary>
        protected string NodeTableName { get; set; }


        /// <inheritdoc/>
        public virtual Task<T2> CreateAsync(T1 entity)
        {
            return queryFactory.Query(NodeTableName)
                .InsertGetIdAsync<T2>(entity);
        }


        /// <inheritdoc/>
        public virtual Task<IEnumerable<T1>> GetAllAsync(PaginationFilter paginationFilter)
        {
            return queryFactory.Query()
                .From(NodeTableName)
                .Offset(paginationFilter.Offset)
                .Limit(paginationFilter.Limit)
                .GetAsync<T1>();
        }


        /// <inheritdoc/>
        public virtual Task<IEnumerable<T1>> GetByIdAsync(T2 id)
        {
            return queryFactory.Query()
                .From(NodeTableName)
                .Where(nameof(BaseModel<object>.Id), id)
                .GetAsync<T1>();
        }


        /// <inheritdoc/>
        public virtual Task<int> UpdateAsync(T2 id, T1 entity)
        {
            return queryFactory.Query(NodeTableName)
                .Where(nameof(BaseModel<object>.Id), id)
                .UpdateAsync(entity);
        }


        /// <inheritdoc/>
        public virtual Task<int> DeleteAsync(T2 id)
        {
            return queryFactory.Query(NodeTableName)
                .Where(nameof(BaseModel<object>.Id), id)
                .DeleteAsync();
        }
    }
}