using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BaseCamp_WEB_API.Core.ExtensionMethods
{
    /// <summary>
    /// Contains an extension method for safe conversion <see cref="IQueryable{T}"/>-objects to <see cref="List{T}"/> ones.
    /// </summary>
    public static class EntityFrameworkSafeToList
    {
        /// <summary>
        /// Converts given <see cref="IQueryable{T}"/> into <see cref="List{T}"/>.
        /// </summary>
        /// <param name="source">Collection of objects to manipulate with.</param>
        /// <typeparam name="T">Type of elements from source.</typeparam>
        /// <returns></returns>
        public static Task<List<T>> ToListSafeAsync<T>(this IQueryable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("Source IQueryable was null!");
            }

            if (!(source is IAsyncEnumerable<T>))
            {
                return Task.FromResult(source.ToList());
            }

            return source.ToListAsync();
        }
    }
}