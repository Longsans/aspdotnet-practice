using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Practice.ViewModels;

namespace Practice.Controllers
{
    [AllowAnonymous]
    public class RegisterController : BaseUserCredentialsController
    {
        private static readonly string _emailErrorKey = "User.Email";

        public IActionResult Index()
        {
            return RedirectToHomeIfAuthenticated(() => View());
        }

        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            return await RedirectToHomeIfAuthenticated(async () =>
            {
                if (model.User == null)
                {
                    model.PageError = "User not set, something went wrong";
                    return View(nameof(Index), model);
                }

                model.UsernameError = ModelState[_usernameErrorKey]?.Errors.FirstOrDefault()?.ErrorMessage;
                model.PasswordError = ModelState[_passwordErrorKey]?.Errors.FirstOrDefault()?.ErrorMessage;
                model.EmailError = ModelState[_emailErrorKey]?.Errors.FirstOrDefault()?.ErrorMessage;

                if (String.IsNullOrWhiteSpace(model.User.Email))
                    model.EmailError = "Email is required";

                if (model.User.Age == null)
                    model.AgeError = "Age is required";

                if (model.UsernameError != null 
                    || model.PasswordError != null
                    || model.EmailError != null
                    || model.AgeError != null)
                    return View(nameof(Index), model);


                var existing = UserService.FindByUsername(model.User.Username);
                if (existing != null)
                {
                    model.UsernameError = "Username is taken";
                    return View(nameof(Index), model);
                }

                await UserService.CreateUser(model.User);
                TempData["Registered"] = true;
                return RedirectToAction(nameof(RegisterSuccess));
            });
        }

        public IActionResult RegisterSuccess()
        {
            if ((bool?)TempData["Registered"] != true)
                return RedirectToAction(nameof(Index));

            return RedirectToHomeIfAuthenticated(() => View());
        }
    }
}
