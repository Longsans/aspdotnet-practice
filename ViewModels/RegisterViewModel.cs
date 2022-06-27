using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Practice.ViewModels
{
    public class RegisterViewModel : BaseUserCredentialsViewModel
    {
        [BindNever]
        public string? EmailError { get; set; }

        [BindNever]
        public string? AgeError { get; set; }
    }
}
