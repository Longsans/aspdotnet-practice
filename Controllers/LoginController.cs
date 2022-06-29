using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FluentValidation;
using Practice.ViewModels;
using Practice.Models;
using Practice.Validators;

namespace Practice.Controllers
{
    public class LoginController : BaseUserCredentialsController
    {
        private readonly IValidator<IUserCredentials> _userCredentialsValidator;

        public LoginController(IValidator<IUserCredentials> userCredentialsValidator)
        {
            _userCredentialsValidator = userCredentialsValidator;
        }

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
                var result = await _userCredentialsValidator.ValidateAsync(
                    model.User,
                    options =>
                    {
                        options.IncludeRuleSets("Username", "LoginPassword");
                    }
                );
                if (!result.IsValid)
                {
                    result.AddToModelState(ModelState, "User");
                    return View(nameof(Index), model);
                }

                if (UserService
                    .FindByUserCredentials(
                        model.User.Username, 
                        model.User.Password) == null)
                {
                    ModelState.AddModelError("", "Username or password is not correct");
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
