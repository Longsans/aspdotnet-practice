using Microsoft.AspNetCore.Mvc.ModelBinding;
using Practice.Models;

namespace Practice.ViewModels
{
    public class BaseUserViewModel : BasePageErrorAndSuccessMessageViewModel
    {
        public User? User { get; set; }
    }
}
