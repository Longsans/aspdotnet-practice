using FluentValidation;
using Practice.Models;

namespace Practice.Validators
{
    public class UserCredentialsValidator : AbstractValidator<IUserCredentials>
    {
        public UserCredentialsValidator()
        {
            RuleSet("Username", () =>
            {
                RuleFor(x => x.Username)
                    .NotEmpty().WithMessage(ValidationMessages.NotEmpty)
                    .Length(4, 20).WithMessage(ValidationMessages.Length);
            });

            RuleSet("LoginPassword", () =>
            {
                RuleFor(x => x.Password)
                    .NotEmpty().WithMessage(ValidationMessages.NotEmpty)
                    .Length(4, 20).WithMessage(ValidationMessages.Length);
            });

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(ValidationMessages.NotEmpty)
                .Length(4, 20).WithMessage(ValidationMessages.Length)
                .NotEqual(x => x.Username).WithMessage(ValidationMessages.NotEqualProperty);
        }
    }
}
