using System.Threading.Tasks;
using BaseCamp_Web_API.Api.Responses.Authentication;
using BaseCamp_Web_API.Api.Services;
using BaseCamp_WEB_API.Core.Entities;

namespace BaseCamp_Web_API.Api.Abstractions.Services
{
    /// <summary>
    /// Contains members to be implemented by <see cref="TokenService"/>.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates a JWT-token/refresh-token pair.
        /// </summary>
        /// <param name="user">User entity to get info from.</param>
        /// <returns></returns>
        public Task<AuthenticationResponse> GenerateTokenPairForUserAsync(User user);

        /// <summary>
        /// Generates a random string of a given length.
        /// </summary>
        /// <param name="length">String length.</param>
        /// <returns>A randomly generated string.</returns>
        public string RandomString(int length);
    }
}