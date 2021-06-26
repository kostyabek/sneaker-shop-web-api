using System.Collections.Generic;
using BaseCamp_Web_API.Api.ServiceAbstractions;
using BaseCamp_WEB_API.Core.Entities;
using BaseCamp_Web_API.Tests.Tests.Controllers;
using Moq;

namespace BaseCamp_Web_API.Tests.Fixtures.Controllers
{
    /// <summary>
    /// Contains needed resources and additional functionality for tests in <see cref="AuthControllerTests"/>.
    /// </summary>
    public class AuthControllerTestsFixture
    {
        /// <summary>
        /// Mocked <see cref="UserService"/>.
        /// </summary>
        public readonly Mock<IUserService> UserService;

        public readonly IEnumerable<User> Users;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthControllerTestsFixture"/> class.
        /// </summary>
        public AuthControllerTestsFixture()
        {
            UserService = new Mock<IUserService>();

            
        }
    }
}