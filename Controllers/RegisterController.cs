using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;
using Practice.Validators;
using Practice.ViewModels;
using Practice.Models;

namespace Practice.Controllers
{
    [AllowAnonymous]
    public class RegisterController : BaseUserCredentialsController
    {
        private static readonly string _emailErrorKey = "User.Email";
        private readonly IValidator<User> _userValidator;

        public RegisterController(IValidator<User> userValidator)
        {
            _userValidator = userValidator;
        }

        public IActionResult Index()
        {
            return RedirectToHomeIfAuthenticated(() => View());
        }

        public async Task<IActionResult> Register(BaseUserViewModel model)
        {
            return await RedirectToHomeIfAuthenticated(async () =>
            {
                var result = await _userValidator.ValidateAsync(
                    model.User,
                    options =>
                    {
                        options
                            .IncludeRuleSets("Username")
                            .IncludeRuleSets("UserInfo")
                            .IncludeRulesNotInRuleSet();
                    }
                );
                if (!result.IsValid)
                {
                    result.AddToModelState(ModelState, "User");
                    return View(nameof(Index), model);
                }

                if (!UserService.ValidateUsername(model.User.Username))
                {
                    ModelState.AddModelError("", "Username is taken");
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
