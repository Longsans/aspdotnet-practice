using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;
using Practice.ViewModels;
using Common.Models;
using Practice.Validators;
using Practice.Services;

namespace Practice.Controllers
{
    public class LoginController : BaseUserCredentialsController
    {
        private readonly IValidator<IUserCredentials> _userCredentialsValidator;
        private readonly IAuthenticationService _authService;

        public LoginController(IValidator<IUserCredentials> userCredentialsValidator, IAuthenticationService authService)
        {
            _userCredentialsValidator = userCredentialsValidator;
            _authService = authService;
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
        [HttpGet]
        [Route("Login2")]
        public IActionResult Login2()
        {
            return RedirectToHomeIfAuthenticated(() =>
            {
                return View();
            });
        }

        [AllowAnonymous]
        public async Task<IActionResult> LogInNoValidate(LoginViewModel model)
        {
            return await RedirectToHomeIfAuthenticated(async () =>
            {
                await _authService.LogIn(
                    model.User.Username,
                    model.User.Password,
                    model.RememberUser,
                    this.UserService,
                    this.HttpContext
                );
                return RedirectToAction("Index", "Home");
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

                if (!await _authService.LogIn(
                        model.User.Username,
                        model.User.Password,
                        model.RememberUser,
                        this.UserService,
                        this.HttpContext
                    ))
                {
                    ModelState.AddModelError("", "Username or password is not correct");
                    return View(nameof(Index), model);
                }
                
                return RedirectToAction("Index", "Home");
            });
        }

        public async Task<IActionResult> Logout()
        {
            await _authService.LogOut(this.HttpContext);
            return RedirectToAction(nameof(Index));
        }
    }
}
