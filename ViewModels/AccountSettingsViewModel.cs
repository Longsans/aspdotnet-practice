using System.ComponentModel.DataAnnotations;
using Practice.Models;

namespace Practice.ViewModels
{
    public class AccountSettingsViewModel : BaseUserViewModel
    {
        [Required(ErrorMessage = "Enter current password")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Re-enter your new password")]
        public string RePassword { get; set; }
    }
}
