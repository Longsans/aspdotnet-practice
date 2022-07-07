using FluentValidation;
using Common.Models;

namespace Common.Validators
{
    public class ContactValidator : AbstractValidator<Contact>
    {
        public ContactValidator()
        {
            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage(ValidationMessages.NotEmpty)
                .Matches(@"^0[1-9]+$").WithMessage("Phone number not in correct format")
                .Length(10, 11).WithMessage("Phone number must be from {MinLength} to {MaxLength} digits");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage(ValidationMessages.NotEmpty)
                .Length(1, 75).WithMessage(ValidationMessages.Length);

            RuleFor(x => x.UserUsername)
                .NotEmpty();
        }
    }
}
