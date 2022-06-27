using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Practice.ViewModels
{
    public class ChangePasswordViewModel : BasePageErrorAndSuccessMessageViewModel
    {
        [BindNever]
        public string? CurrentPasswordError { get; set; }

        [BindNever]
        public string? NewPasswordError { get; set; }

        [BindNever]
        public string? RePasswordError { get; set; }

        [Required(ErrorMessage = "Enter current password")]
        [StringLength
            (20,
            MinimumLength = 4,
            ErrorMessage = "Password must be between {2} and {1} characters")]
        public string? CurrentPassword { get; set; }

        [Required(ErrorMessage = "Enter your new password")]
        [StringLength
            (20,
            MinimumLength = 4,
            ErrorMessage = "Password must be between {2} and {1} characters")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Re-enter your new password")]
        public string? RePassword { get; set; }
    }
}
