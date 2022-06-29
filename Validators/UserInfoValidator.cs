using FluentValidation;
using Practice.Models;

namespace Practice.Validators
{
    public class UserInfoValidator : AbstractValidator<IUserInfo>
    {
        public UserInfoValidator()
        {
            RuleSet("UserInfo", () =>
            {
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
