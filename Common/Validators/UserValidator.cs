using FluentValidation;
using Common.Models;

namespace Common.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator(
            IValidator<IUserInfo> userInfoValidator, 
            IValidator<IUserCredentials> userCredentialsValidator)
        {
            Include(userInfoValidator);
            Include(userCredentialsValidator);
        }
    }
}
