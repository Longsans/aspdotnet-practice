using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Practice.ViewModels;
using Practice.Utilities;
using System.Security.Claims;

namespace Practice.Controllers
{
    public class LoginController : BaseUserCredentialsController
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            return RedirectToHomeIfAuthenticated(() =>
            {
                var model = new LoginViewModel
                {
                    PageError = (string?)TempData["Error"],
                    SuccessMessage = (string?)TempData["SuccessMessage"]
                };
                return View(model);
            });
        }

        [AllowAnonymous]
        public async Task<IActionResult> Authenticate(LoginViewModel model)
        {
            return await RedirectToHomeIfAuthenticated(async () =>
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


                if (UserService
                    .FindByUserCredentials(
                        model.User.Username, 
                        model.User.Password) == null)
                {
                    model.PageError = "Username or password is not correct";
                    return View(nameof(Index), model);
                }

                var claimsIdentity = new ClaimsIdentity(
                    new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.User.Username)
                    },
                    CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(claimsIdentity);
                var authProps = new AuthenticationProperties
                {
                    IsPersistent = true
                };
                if (!model.RememberUser)
                {
                    authProps.IsPersistent = false;
                    authProps.ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1);
                    authProps.AllowRefresh = false;
                }

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    authProps
                );
                return RedirectToAction("Index", "Home");
            });
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }
    }
}
