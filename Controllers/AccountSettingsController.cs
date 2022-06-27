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
        private static readonly string _ageErrorKey = "UserInfo.User.Age";

        public IActionResult Index()
        {
            return RedirectToLogin(() =>
            {
                var user = UserService.FindByUsername(
                    HttpContext.Session.GetString(_sessionKeyName)!
                );
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
            });
        }

        public async Task<IActionResult> SaveUserInfo(AccountSettingsViewModel model)
        {
            return await RedirectToLogin(async () =>
            {
                var userInfo = model.UserInfo!;
                if (userInfo.User == null)
                {
                    userInfo.PageError = "User not set, something went wrong";
                    return View(nameof(Index), model);
                }
                userInfo.User.Username = HttpContext.Session.GetString(_sessionKeyName)!;

                bool invalid = false;
                if (String.IsNullOrWhiteSpace(userInfo.User.Email))
                {
                    userInfo.EmailError = "Email is required";
                    invalid = true;
                }
                if (userInfo.User.Age == null)
                {
                    userInfo.AgeError = "Age is required";
                    invalid = true;
                }
                if (invalid)
                    return View(nameof(Index), model);

                userInfo.EmailError = ModelState[_emailErrorKey]?.Errors.FirstOrDefault()?.ErrorMessage;
                if (userInfo.EmailError != null)
                    return View(nameof(Index), model);

                await UserService.UpdateUserInfo(userInfo.User);
                userInfo.SuccessMessage = "Update personal info success!";
                return View(nameof(Index), model);
            });
        }

        public async Task<IActionResult> SavePassword(AccountSettingsViewModel model)
        {
            return await RedirectToLogin(async () =>
            {
                var changePwd = model.ChangePassword!;
                model.UserInfo!.User!.Username = HttpContext.Session.GetString(_sessionKeyName)!;

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
                changePwd.SuccessMessage = "Change password success!";
                return View(nameof(Index), model);
            });
        }
    }
}
