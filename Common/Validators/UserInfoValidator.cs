using FluentValidation;
using Common.Models;

namespace Common.Validators
{
    public class UserInfoValidator : AbstractValidator<IUserInfo>
    {
        public UserInfoValidator()
        {
            RuleSet("UserInfo", () =>
            {
                RuleFor(x => x.FirstName)
                    .NotEmpty().WithMessage(ValidationMessages.NotEmpty)
                    .Length(1, 20).WithMessage(ValidationMessages.Length);

                RuleFor(x => x.LastName)
                    .NotEmpty().WithMessage(ValidationMessages.NotEmpty)
                    .Length(1, 20).WithMessage(ValidationMessages.Length);

                RuleFor(x => x.Email)
                    .NotEmpty().WithMessage(ValidationMessages.NotEmpty)
                    .MaximumLength(30).WithMessage(ValidationMessages.MaxLength)
                    .EmailAddress().WithMessage(ValidationMessages.Email);

                RuleFor(x => x.Age)
                    .NotNull().WithMessage(ValidationMessages.NotEmpty)
                    .InclusiveBetween(0, 200).WithMessage(ValidationMessages.Inclusive);
            });
        }
    }
}
