namespace BaseCamp_Web_API.Api.Validators.ErrorMessages
{
    /// <summary>
    /// Contains error messages for RegistrationRequestValidator.
    /// </summary>
    public static class RegistrationRequestValidatorMessages
    {
        /// <summary>
        /// Error message for username.
        /// </summary>
        public const string UsernameFormatNotValid = "Username must contain letters, numbers, underscores or dots and be from 4 to 20 characters long.";

        /// <summary>
        /// Error message for password.
        /// </summary>
        public const string PasswordFormatNotValid =
            "Password must consist of lower and uppercase letters, at least one number, one special character(@$!%*?&) and be 8+ characters long";

        /// <summary>
        /// Error message for email.
        /// </summary>
        public const string WrongEmailFormat = "Wrong email format!";

        /// <summary>
        /// Error message for first name.
        /// </summary>
        public const string FirstNameWrongFormat = "First name must be between 2 and 15.";

        /// <summary>
        /// Error message for user role ID.
        /// </summary>
        public const string UserRoleIdNotFound = "User role ID not found.";

        /// <summary>
        /// Error message for last name.
        /// </summary>
        public const string LastNameWrongFormat = "Last name must be between 2 and 15";

        /// <summary>
        /// Error message for patronymic.
        /// </summary>
        public const string PatronymicWrongFormat = "Patronymic must be between 6 and 15";

        /// <summary>
        /// Error message for user age.
        /// </summary>
        public const string UserAgeInvalidAge = "User age must be between 16 and 120";

        /// <summary>
        /// Error message for gender ID.
        /// </summary>
        public const string GenderIdNotFound = "Gender ID not found";
    }
}