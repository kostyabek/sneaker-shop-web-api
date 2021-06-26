using BaseCamp_Web_API.Api.Validators;
using BaseCamp_Web_API.Tests.Validators;

namespace BaseCamp_Web_API.Tests.Fixtures.Validators
{
    /// <summary>
    /// Contains needed resources and additional functionality for tests in <see cref="UserRegistrationRequestValidatorTests"/>>.
    /// </summary>
    public class UserRegistrationRequestValidatorTestsFixture
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRegistrationRequestValidatorTestsFixture"/> class.
        /// </summary>
        public UserRegistrationRequestValidatorTestsFixture()
        {
            Validator = new UserRegistrationRequestValidator();
        }

        /// <summary>
        /// Gets or sets <see cref="UserRegistrationRequestValidator"/>.
        /// </summary>
        public UserRegistrationRequestValidator Validator { get; set; }
    }
}