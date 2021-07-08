using BaseCamp_Web_API.Api.Requests.Authentication;
using BaseCamp_Web_API.Api.Validators.ErrorMessages;
using FluentValidation;

namespace BaseCamp_Web_API.Api.Validators
{
    /// <summary>
    /// Validates User.
    /// </summary>
    public class UserRegistrationRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRegistrationRequestValidator"/> class.
        /// </summary>
        public UserRegistrationRequestValidator()
        {
            RuleFor(u => u.Username)
                .Matches("^(?=.{4,20}$)(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$")
                .WithMessage(RegistrationRequestValidatorMessages.UsernameFormatNotValid);

            RuleFor(u => u.Password)
                .Matches("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$")
                .WithMessage(RegistrationRequestValidatorMessages.PasswordFormatNotValid);

            RuleFor(u => u.Email)
                .Matches("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$")
                .WithMessage(RegistrationRequestValidatorMessages.WrongEmailFormat);

            RuleFor(u => u.FirstName)
                .Matches("^[A-Za-z]{2,15}$")
                .WithMessage(RegistrationRequestValidatorMessages.FirstNameWrongFormat);

            RuleFor(u => u.LastName)
                .Matches("^[A-Za-z]{2,15}$")
                .WithMessage(RegistrationRequestValidatorMessages.LastNameWrongFormat);

            RuleFor(u => u.Patronymic)
                .Matches("^[A-Za-z]{6,15}$")
                .WithMessage(RegistrationRequestValidatorMessages.PatronymicWrongFormat);

            RuleFor(u => u.Age)
                .InclusiveBetween(16, 120)
                .WithMessage(RegistrationRequestValidatorMessages.UserAgeInvalidAge);

            RuleFor(u => u.GenderId)
                .InclusiveBetween(1, 2)
                .WithMessage(RegistrationRequestValidatorMessages.GenderIdNotFound);
        }
    }
}