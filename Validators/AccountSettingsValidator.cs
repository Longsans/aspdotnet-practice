using FluentValidation;
using Practice.Models;
using Practice.ViewModels;

namespace Practice.Validators
{
    public class AccountSettingsValidator : AbstractValidator<AccountSettingsViewModel>
    {
        public AccountSettingsValidator(IValidator<User> userValidator)
        {
            RuleFor(x => x.User).SetValidator(userValidator);

            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage(ValidationMessages.NotEmpty)
                .Length(4, 20).WithMessage(ValidationMessages.Length).WithName("Password");

            RuleFor(x => x.User.Password)
                .NotEqual(x => x.CurrentPassword).WithMessage("New password cannot be the same as current password");

            RuleFor(x => x.RePassword)
                .NotEmpty().WithMessage(ValidationMessages.NotEmpty)
                .Equal(x => x.User.Password).WithMessage(ValidationMessages.EqualPropertyPwd).WithName("Re-entered password");
        }
    }
}
