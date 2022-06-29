using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Practice.ViewModels
{
    public interface IPageErrorAndSuccessMessage
    {
        [BindNever]
        public string? PageError { get; set; }

        [BindNever]
        public string? SuccessMessage { get; set; }
    }
}
