using System.Collections.Generic;
using BaseCamp_WEB_API.Core.Entities;

namespace BaseCamp_WEB_API.Core.RepositoryAbstractions
{
    /// <summary>
    /// Contains extra method signatures to be implemented by SneakerRepository.
    /// </summary>
    public interface ISneakerRepository : IGenericRepository<Sneaker, int>
    {
    }
}