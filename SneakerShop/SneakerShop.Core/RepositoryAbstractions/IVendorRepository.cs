using BaseCamp_WEB_API.Core.Entities;

namespace BaseCamp_WEB_API.Core.RepositoryAbstractions
{
    /// <summary>
    /// Contains extra method signatures to be implemented by VendorRepository.
    /// </summary>
    public interface IVendorRepository : IGenericRepository<Vendor, int>
    {
    }
}