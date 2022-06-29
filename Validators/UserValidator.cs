using FluentValidation;
using Practice.Models;

namespace Practice.Validators
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
