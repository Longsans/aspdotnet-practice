using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Practice.ViewModels
{
    public class LoginViewModel : BaseUserViewModel
    {
        [BindNever]
        public string? UsernameError { get; set; }

        [BindNever]
        public string? PasswordError { get; set; }
    }
}
