using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Practice.ViewModels
{
    public class UserInfoViewModel : BaseUserViewModel
    {
        [BindNever]
        public string? EmailError { get; set; }

        [BindNever]
        public string? AgeError { get; set; }
    }
}
