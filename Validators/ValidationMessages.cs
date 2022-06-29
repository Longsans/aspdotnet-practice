namespace Practice.Validators
{
    public static class ValidationMessages
    {
        private const string _propName = "{PropertyName}";
        private const string _maxLength = "{MaxLength}";

        public const string NotEmpty = $"{_propName} must not be empty";
        public const string Length = $"{_propName} must be between {{MinLength}} and {_maxLength} characters";
        public const string EqualPropertyPwd = $"{_propName} does not match new password";
        public const string NotEqualProperty = $"{_propName} cannot be the same as {{ComparisonProperty}}";
        public const string MaxLength = $"{_propName} must be {_maxLength} characters or fewer";
        public const string Email = $"{_propName} is not a valid email address";
        public const string Inclusive = $"{_propName} must be between {{From}} and {{To}}";
    }
}
