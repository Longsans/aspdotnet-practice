using Microsoft.AspNetCore.Mvc;
using Practice.ViewModels;

namespace Practice.Controllers
{
    public class RegisterController : BaseUserCredentialsController
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Register(LoginViewModel model)
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
            if (model.User.Username.ToLower() == model.User.Password.ToLower())
            {
                model.PageError = "Password cannot be the same as username!";
                return View(nameof(Index), model);
            }

            var existing = UserService.FindByUsername(model.User.Username);
            if (existing != null)
            {
                model.UsernameError = "Username is taken";
                return View(nameof(Index), model);
            }

            await UserService.CreateUser(model.User);
            return RedirectToAction(nameof(RegisterSuccess));
        }

        public IActionResult RegisterSuccess()
        {
            return View();
        }
    }
}
