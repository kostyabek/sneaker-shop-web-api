using BaseCamp_Web_API.Api.Requests.Authentication;
using BaseCamp_Web_API.Api.Validators.ErrorMessages;
using BaseCamp_Web_API.Tests.Fixtures.Validators;
using FluentAssertions;
using Xunit;

namespace BaseCamp_Web_API.Tests.Tests.Validators
{
    /// <summary>
    /// Contains unit tests for RegistrationRequestValidator.
    /// </summary>
    public class UserRegistrationRequestValidatorTests : IClassFixture<UserRegistrationRequestValidatorTestsFixture>
    {
        private readonly UserRegistrationRequestValidatorTestsFixture _fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRegistrationRequestValidatorTests"/> class.
        /// </summary>
        /// <param name="fixture">Contains resources for tests.</param>
        public UserRegistrationRequestValidatorTests(UserRegistrationRequestValidatorTestsFixture fixture)
        {
            _fixture = fixture;
        }

        /// <summary>
        /// Tests validator response when username of a new user is invalid.
        /// </summary>
        /// <param name="username">Test value for username.</param>
        [Theory]
        [InlineData("")]
        [InlineData("te")]
        [InlineData("test!")]
        [InlineData("abcd Efghijklmnop")]
        [InlineData("__1234")]
        [InlineData("1234..")]
        [InlineData("_.1234")]
        [InlineData("1234_.")]
        public void
            UserRegistrationRequestValidator_CreatingNewUserAndUsernameIsInvalid_ErrorAboutUsernameWrongFormat(string username)
        {
            // Arrange
            var registerUserRequest = new RegisterUserRequest
            {
                Username = username,
                Password = "Superpass123!",
                Age = 20,
                Email = "test@mail.com",
                FirstName = "Mykola",
                LastName = "Kolos",
                Patronymic = "Andriyevych",
                GenderId = 1
            };

            // Act
            var validationResult = _fixture.Validator.Validate(registerUserRequest);

            // Assert
            validationResult.Errors.Count.Should().Be(1);
            validationResult.Errors[0].ErrorMessage.Should().Be(RegistrationRequestValidatorMessages.UsernameFormatNotValid);
        }

        /// <summary>
        /// Tests validator response when password of a new user is invalid.
        /// </summary>
        /// <param name="password">Test value for password.</param>
        [Theory]
        [InlineData("")]
        [InlineData("Tt123$!")]
        [InlineData("Testpassw1")]
        [InlineData("5uperPassw0rd^")]
        public void
            UserRegistrationRequestValidator_CreatingNewUserAndPasswordIsInvalid_ErrorAboutPasswordWrongFormat(string password)
        {
            // Arrange
            var registerUserRequest = new RegisterUserRequest
            {
                Username = "username",
                Password = password,
                Age = 20,
                Email = "test@mail.com",
                FirstName = "Mykola",
                LastName = "Kolos",
                Patronymic = "Andriyevych",
                GenderId = 1
            };

            // Act
            var validationResult = _fixture.Validator.Validate(registerUserRequest);

            // Assert
            validationResult.Errors.Count.Should().Be(1);
            validationResult.Errors[0].ErrorMessage.Should().Be(RegistrationRequestValidatorMessages.PasswordFormatNotValid);
        }

        /// <summary>
        /// Tests validator response when age of a new user is invalid.
        /// </summary>
        /// <param name="age">Test value for age.</param>
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(125)]
        public void UserRegistrationRequestValidator_CreatingNewUserAndAgeIsInvalid_ErrorAboutInvalidAge(int age)
        {
            // Arrange
            var registerUserRequest = new RegisterUserRequest
            {
                Username = "username",
                Password = "19!P4ssWd$92",
                Age = age,
                Email = "test@mail.com",
                FirstName = "Mykola",
                LastName = "Kolos",
                Patronymic = "Andriyevych",
                GenderId = 1
            };

            // Act
            var validationResult = _fixture.Validator.Validate(registerUserRequest);

            // Assert
            validationResult.Errors.Count.Should().Be(1);
            validationResult.Errors[0].ErrorMessage.Should().Be(RegistrationRequestValidatorMessages.UserAgeInvalidAge);
        }

        /// <summary>
        /// Tests validator response when email of a new user is invalid.
        /// </summary>
        /// <param name="email">Test value for email.</param>
        [Theory]
        [InlineData("")]
        [InlineData("@")]
        [InlineData("k@a")]
        [InlineData("k@a.s")]
        [InlineData("ko@ok@ok.com")]
        public void
            UserRegistrationRequestValidator_CreatingNewUserAndEmailIsInvalid_ErrorAboutInvalidEmail(string email)
        {
            // Arrange
            var registerUserRequest = new RegisterUserRequest
            {
                Username = "username",
                Password = "19!P4ssWd$92",
                Age = 20,
                Email = email,
                FirstName = "Mykola",
                LastName = "Kolos",
                Patronymic = "Andriyevych",
                GenderId = 1
            };

            // Act
            var validationResult = _fixture.Validator.Validate(registerUserRequest);

            // Assert
            validationResult.Errors.Count.Should().Be(1);
            validationResult.Errors[0].ErrorMessage.Should().Be(RegistrationRequestValidatorMessages.WrongEmailFormat);
        }

        /// <summary>
        /// Tests validator response when first and last names of a new user are invalid.
        /// </summary>
        /// <param name="name">Test value for first and last names.</param>
        [Theory]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("k")]
        [InlineData("k123")]
        [InlineData("TestTestTestTestTestTestTestTest")]
        public void
            UserRegistrationRequestValidator_CreatingNewUserAndFirstAndLastNamesAreInvalid_ErrorAboutInvalidNames(string name)
        {
            // Arrange
            var registerUserRequest = new RegisterUserRequest
            {
                Username = "username",
                Password = "19!P4ssWd$92",
                Age = 20,
                Email = "email@test.com",
                FirstName = name,
                LastName = name,
                Patronymic = "Andriyevych",
                GenderId = 1
            };

            // Act
            var validationResult = _fixture.Validator.Validate(registerUserRequest);

            // Assert
            validationResult.Errors.Count.Should().Be(2);
            validationResult.Errors[0].ErrorMessage.Should().Be(RegistrationRequestValidatorMessages.FirstNameWrongFormat);
            validationResult.Errors[1].ErrorMessage.Should().Be(RegistrationRequestValidatorMessages.LastNameWrongFormat);
        }

        /// <summary>
        /// Tests validator response when patronymic of a new user is invalid.
        /// </summary>
        /// <param name="patronymic">Test value for patronymic.</param>
        [Theory]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("k")]
        [InlineData("k123kkk")]
        [InlineData("TestTestTestTestTestTestTestTest")]
        public void
            UserRegistrationRequestValidator_CreatingNewUserAndPatronymicIsInvalid_ErrorAboutInvalidPatronymic(string patronymic)
        {
            // Arrange
            var registerUserRequest = new RegisterUserRequest
            {
                Username = "username",
                Password = "19!P4ssWd$92",
                Age = 20,
                Email = "email@test.com",
                FirstName = "Kostya",
                LastName = "Bondar",
                Patronymic = patronymic,
                GenderId = 1
            };

            // Act
            var validationResult = _fixture.Validator.Validate(registerUserRequest);

            // Assert
            validationResult.Errors.Count.Should().Be(1);
            validationResult.Errors[0].ErrorMessage.Should().Be(RegistrationRequestValidatorMessages.PatronymicWrongFormat);
        }

        /// <summary>
        /// Tests validator response when gender ID of a new user is invalid.
        /// </summary>
        /// <param name="genderId">Test value for gender ID.</param>
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(5)]
        public void
            UserRegistrationRequestValidator_CreatingNewUserAndGenderIdIsInvalid_ErrorAboutInvalidGenderId(int genderId)
        {
            // Arrange
            var registerUserRequest = new RegisterUserRequest
            {
                Username = "username",
                Password = "19!P4ssWd$92",
                Age = 20,
                Email = "email@test.com",
                FirstName = "Kostya",
                LastName = "Bondar",
                Patronymic = "Mykolayovych",
                GenderId = genderId
            };

            // Act
            var validationResult = _fixture.Validator.Validate(registerUserRequest);

            // Assert
            validationResult.Errors.Count.Should().Be(1);
            validationResult.Errors[0].ErrorMessage.Should().Be(RegistrationRequestValidatorMessages.GenderIdNotFound);
        }
    }
}