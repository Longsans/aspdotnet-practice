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
        public IActionResult Index(bool loginRequired = false)
        {
            return RedirectToHomeIfAuthenticated(() =>
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


                var existing = UserService.FindByUsername(model.User.Username);
                var hashedPwd = Encrypter.EncryptSHA256(model.User.Password);
                if (existing == null || existing.Password != hashedPwd)
                {
                    model.PageError = "Username or password is not correct";
                    return View(nameof(Index), model);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, existing.Username)
                };
                var claimsIdentity = new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(claimsIdentity);
                var authProps = new AuthenticationProperties
                {
                    IsPersistent = true
                };
                if (model.RememberUser != true)
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
