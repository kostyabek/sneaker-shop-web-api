using System.Threading.Tasks;
using BaseCamp_Web_API.Api.Requests.Authentication;
using BaseCamp_Web_API.Api.Responses.Authentication;

namespace BaseCamp_Web_API.Api.ServiceAbstractions
{
    /// <summary>
    /// Contains members to be implemented by UserService.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Registers new users asynchronously.
        /// </summary>
        /// <param name="request">Contains user registration info.</param>
        /// <returns>A <see cref="Task{TResult}"/> with access and refresh tokens. If errors occured, then the list of them.</returns>
        Task<AuthenticationResponse> RegisterAsync(RegisterUserRequest request);

        /// <summary>
        /// Log-ins existing users asynchronously.
        /// </summary>
        /// <param name="request">Contains user log-in credentials.</param>
        /// <returns>A <see cref="Task{TResult}"/> with access and refresh tokens. If errors occured, then the list of them.</returns>
        Task<AuthenticationResponse> LoginAsync(LoginUserRequest request);

        /// <summary>
        /// Renews refresh token for a user.
        /// </summary>
        /// <param name="request">Token data.</param>
        /// <returns>A <see cref="Task{TResult}"/> with access and refresh tokens. If errors occured, then the list of them.</returns>
        Task<AuthenticationResponse> RefreshTokenAsync(RefreshTokenRequest request);
    }
}