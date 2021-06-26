using BaseCamp_WEB_API.Core.Entities;

namespace BaseCamp_WEB_API.Core.RepositoryAbstractions
{
    /// <summary>
    /// Contains extra method signatures to be implemented by SeasonRepository.
    /// </summary>
    public interface ISeasonRepository : IGenericRepository<Season, int>
    {
    }
}