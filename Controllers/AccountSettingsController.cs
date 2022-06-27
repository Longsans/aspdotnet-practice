using Microsoft.AspNetCore.Mvc;
using Practice.ViewModels;
using Practice.Utilities;

namespace Practice.Controllers
{
    public class AccountSettingsController : BaseUserCredentialsController
    {
        private static readonly string _currentPasswordErrorKey = "ChangePassword.CurrentPassword";
        private static readonly string _newPasswordErrorKey = "ChangePassword.NewPassword";
        private static readonly string _rePasswordErrorKey = "ChangePassword.RePassword";
        private static readonly string _emailErrorKey = "UserInfo.User.Email";

        public IActionResult Index()
        {
            var user = UserService.FindByUsername(HttpContext.User.Identity!.Name!);
            if (user == null)
                return View("Error");

            var model = new AccountSettingsViewModel
            {
                UserInfo = new UserInfoViewModel
                {
                    User = user
                },
                ChangePassword = new ChangePasswordViewModel()
            };
            return View(model);
        }

        public async Task<IActionResult> SaveUserInfo(AccountSettingsViewModel model)
        {
            var userInfo = model.UserInfo!;
            if (userInfo.User == null)
            {
                userInfo.PageError = "User not set, something went wrong";
                return View(nameof(Index), model);
            }
            userInfo.User.Username = HttpContext.User.Identity!.Name!;

            userInfo.EmailError = ModelState[_emailErrorKey]?.Errors.FirstOrDefault()?.ErrorMessage;
            if (String.IsNullOrWhiteSpace(userInfo.User.Email))
                userInfo.EmailError = "Email is required";

            if (userInfo.User.Age == null)
                userInfo.AgeError = "Age is required";

            if (userInfo.EmailError != null || userInfo.AgeError != null)
                return View(nameof(Index), model);

            await UserService.UpdateUserInfo(userInfo.User);
            userInfo.SuccessMessage = "Update personal info success!";
            return View(nameof(Index), model);
        }

        public async Task<IActionResult> SavePassword(AccountSettingsViewModel model)
        {
            var changePwd = model.ChangePassword!;
            model.UserInfo!.User!.Username = HttpContext.User.Identity!.Name!;

            changePwd.CurrentPasswordError = ModelState[_currentPasswordErrorKey]?.Errors.FirstOrDefault()?.ErrorMessage;
            changePwd.NewPasswordError = ModelState[_newPasswordErrorKey]?.Errors.FirstOrDefault()?.ErrorMessage;
            changePwd.RePasswordError = ModelState[_rePasswordErrorKey]?.Errors.FirstOrDefault()?.ErrorMessage;
            if (changePwd.CurrentPasswordError != null || changePwd.NewPasswordError != null || changePwd.RePasswordError != null)
                return View(nameof(Index), model);

            var user = UserService.FindByUsername(model.UserInfo.User.Username);
            if (user == null)
            {
                return View("Error");
            }
            if (user.Password != Encrypter.EncryptSHA256(changePwd.CurrentPassword!))
            {
                changePwd.CurrentPasswordError = "Entered password doesn't match current password";
                return View(nameof(Index), model);
            }
            if (changePwd.CurrentPassword == changePwd.NewPassword)
            {
                changePwd.NewPasswordError = "New password cannot be the same as current password";
                return View(nameof(Index), model);
            }
            if (changePwd.NewPassword != changePwd.RePassword)
            {
                changePwd.RePasswordError = changePwd.NewPasswordError = "Passwords do not match";
                return View(nameof(Index), model);
            }

            user.Password = changePwd.NewPassword!;
            await UserService.UpdateUserPassword(user);

            TempData["SuccessMessage"] = "Your password has been changed successfully, please log in again";
            return RedirectToAction("Logout", "Login");
        }
    }
}
