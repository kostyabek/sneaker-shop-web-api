using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BaseCamp_Web_API.Tests
{
    /// <summary>
    /// Helps creating mocked <see cref="DbContext"/>-objects for unit testing.
    /// </summary>
    public class DbContextMockHelper
    {
        /// <summary>
        /// Creates mocked <see cref="DbSet{TEntity}"/>-objects for unit testing.
        /// </summary>
        /// <param name="source">Collection of data to import into mocked <see cref="DbSet{TEntity}"/>.</param>
        /// <typeparam name="T">Type of <see cref="DbSet{TEntity}"/> contents.</typeparam>
        /// <returns></returns>
        public static DbSet<T> GetQueryableMockDbSet<T>(List<T> source) where T : class
        {
            var dbSet = new Mock<DbSet<T>>();
            var queryable = source.AsQueryable();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            dbSet.Setup(s => s.Add(It.IsAny<T>())).Callback<T>(source.Add);
            return dbSet.Object;
        }
    }
}