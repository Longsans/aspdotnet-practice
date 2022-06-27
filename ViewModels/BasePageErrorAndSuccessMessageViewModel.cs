using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Practice.ViewModels
{
    public class BasePageErrorAndSuccessMessageViewModel
    {
        [BindNever]
        public string? PageError { get; set; }

        [BindNever]
        public string? SuccessMessage { get; set; }
    }
}
