using System.Collections.Generic;
using System.Threading.Tasks;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_WEB_API.Core.Filters;

namespace BaseCamp_WEB_API.Core.RepositoryAbstractions
{
    /// <summary>
    /// "Data Access Object" abstraction for DI-container.
    /// </summary>
    /// <typeparam name="T1">Entity type that is going to be used by implementation.</typeparam>
    /// <typeparam name="T2">Data-type of ID.</typeparam>
    public interface IGenericRepository<T1, T2> where T1 : BaseModel<T2>
    {
        /// <summary>
        /// Creates a record.
        /// </summary>
        /// <param name="entity">Contains data to send to a database.</param>
        /// <returns>A <see cref="Task{TResult}"/> with ID of the created record.</returns>
        Task<T2> CreateAsync(T1 entity);

        /// <summary>
        /// Fetches all of the records.
        /// </summary>
        /// <param name="paginationFilter">Contains offset and limit for a database query.</param>
        /// <returns>
        /// A <see cref="Task"/> with <see cref="IEnumerable{T}"/> with queried records.
        /// </returns>
        Task<IEnumerable<T1>> GetAllAsync(PaginationFilter paginationFilter);

        /// <summary>
        /// Fetches a record with the given ID.
        /// </summary>
        /// <param name="id">ID of a record to fetch.</param>
        /// <returns>
        /// A <see cref="Task"/> with <see cref="IEnumerable{T}"/> with queried records.
        /// </returns>
        Task<IEnumerable<T1>> GetByIdAsync(T2 id);

        /// <summary>
        /// Updates a record with the given ID.
        /// </summary>
        /// <param name="id">ID of a record to update.</param>
        /// <param name="entity">Contains new data.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> with ID of the updated record.
        /// </returns>
        Task<int> UpdateAsync(T2 id, T1 entity);

        /// <summary>
        /// Deletes a record with the given ID.
        /// </summary>
        /// <param name="id">ID of a record to delete.</param>
        /// <returns>A <see cref="Task{TResult}"/> with ID of the deleted record.</returns>
        Task<int> DeleteAsync(T2 id);
    }
}