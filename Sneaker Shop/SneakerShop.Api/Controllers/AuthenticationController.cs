using System.Threading.Tasks;
using BaseCamp_Web_API.Api.Requests.Authentication;
using BaseCamp_Web_API.Api.Responses.Authentication;
using BaseCamp_Web_API.Api.ServiceAbstractions;
using Microsoft.AspNetCore.Mvc;

namespace BaseCamp_Web_API.Api.Controllers
{
    /// <summary>
    /// Controller for authentication processes.
    /// </summary>
    [ApiController]
    [Route("api/v1.0/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationController"/> class.
        /// </summary>
        /// <param name="userService">For user-specific utility operations.</param>
        public AuthenticationController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Registers new users.
        /// </summary>
        /// <param name="request">Request to be processed.</param>
        /// <returns><see cref="AuthenticationResponse"/> in <see cref="OkObjectResult"/> or <see cref="ControllerBase.BadRequest()"/>.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserRequest request)
        {
            var response = await _userService.RegisterAsync(request);
            if (!response.Success)
            {
                return BadRequest(new AuthenticationResponse
                {
                    Success = false,
                    Errors = response.Errors
                });
            }

            return Ok(new RegisterUserResponse
            {
                Success = true,
                Token = response.Token,
                RefreshToken = response.RefreshToken
            });
        }

        /// <summary>
        /// Performs log-in operation for existing users.
        /// </summary>
        /// <param name="request">Authentication data to process.</param>
        /// <returns><see cref="AuthenticationResponse"/> in <see cref="OkObjectResult"/> or <see cref="ControllerBase.BadRequest()"/>.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginUserRequest request)
        {
            var response = await _userService.LoginAsync(request);
            if (!response.Success)
            {
                return BadRequest(new AuthenticationResponse
                {
                    Success = false,
                    Errors = response.Errors
                });
            }

            return Ok(new AuthenticationResponse
            {
                Success = true,
                Token = response.Token,
                RefreshToken = response.RefreshToken
            });
        }

        /// <summary>
        /// Refreshes tokens.
        /// </summary>
        /// <param name="refreshTokenRequest">Token data needed for a request.</param>
        /// <returns><see cref="AuthenticationResponse"/> in <see cref="OkObjectResult"/> or <see cref="ControllerBase.BadRequest()"/>.</returns>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var response = await _userService.RefreshTokenAsync(refreshTokenRequest);

            if (!response.Success)
            {
                return BadRequest(new RegisterUserResponse
                {
                    Errors = response.Errors,
                    Success = false
                });
            }

            return Ok(response);
        }
    }
}