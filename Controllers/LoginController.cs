using Microsoft.AspNetCore.Mvc;
using Practice.ViewModels;
using Practice.Utilities;

namespace Practice.Controllers
{
    public class LoginController : BaseUserCredentialsController
    {
        public IActionResult Index(bool loginRequired = false)
        {
            if (loginRequired)
            {
                var model = new LoginViewModel
                {
                    PageError = "You must login first"
                };
                return View(model);
            }
            return View();
        }

        public IActionResult Authenticate(LoginViewModel model)
        {
            if (model.User == null)
            {
                model.PageError = "User not set, something went wrong";
                return View(nameof(Index), model);
            }
            if (!ModelState.IsValid)
            {
                model.UsernameError = ModelState[_usernameErrorKey]?.Errors.FirstOrDefault()?.ErrorMessage;
                model.PasswordError = ModelState[_passwordErrorKey]?.Errors.FirstOrDefault()?.ErrorMessage;
                return View(nameof(Index), model);
            }
            

            var existing = UserService.FindByUsername(model.User.Username);
            var hashedPwd = Encrypter.EncryptSHA256(model.User.Password);
            if (existing == null || existing.Password != hashedPwd)
            {
                model.PageError = "Username or password is not correct";
                return View(nameof(Index), model);
            }

            HttpContext.Session.SetString(_sessionKeyName, model.User.Username);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Index));
        }
    }
}
