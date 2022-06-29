using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Practice.ViewModels;
using Practice.Models;
using Practice.Validators;

namespace Practice.Controllers
{
    public class AccountSettingsController : BaseUserCredentialsController
    {
        private readonly IValidator<IUserInfo> _userInfoValidator;
        private readonly IValidator<AccountSettingsViewModel> _accountSettingsValidator;

        public AccountSettingsController(IValidator<IUserInfo> userInfoValidator, IValidator<AccountSettingsViewModel> accountSettingsValidator)
        {
            _userInfoValidator = userInfoValidator;
            _accountSettingsValidator = accountSettingsValidator;
        }

        public IActionResult Index()
        {
            var user = UserService.FindByUsername(HttpContext.User.Identity.Name);
            if (user == null)
                return View("Error");

            var model = new AccountSettingsViewModel
            {
                User = user
            };
            return View(model);
        }

        public async Task<IActionResult> SaveUserInfo(AccountSettingsViewModel model)
        {
            ModelState.Clear();
            var result = await _userInfoValidator.ValidateAsync(
                model.User, 
                options =>
                {
                    options.IncludeRuleSets("UserInfo");
                }
            );
            if (!result.IsValid)
            {
                result.AddToModelState(ModelState, "User");
                return View(nameof(Index), model);
            }

            await UserService.UpdateUserInfo(new User(model.User));
            model.SuccessMessage = "Update personal info success!";
            return View(nameof(Index), model);
        }

        public async Task<IActionResult> SavePassword(AccountSettingsViewModel model)
        {
            ModelState.Clear();
            var result = await _accountSettingsValidator.ValidateAsync(
                model,
                options =>
                {
                    options.IncludeRulesNotInRuleSet();
                }
            );
            if (!result.IsValid)
            {
                result.AddToModelStatePrefixed(ModelState, new Dictionary<string, string>
                {
                    { "Password", "User" }
                });

                return View(nameof(Index), model);
            }
            if (UserService
                .FindByUserCredentials(
                    model.User.Username, 
                    model.CurrentPassword) == null)
            {
                ModelState.AddModelError("CurrentPassword", "Entered password does not match current password");
                return View(nameof(Index), model);
            }

            await UserService.UpdateUserPassword(
                new User
                {
                    Username = model.User.Username,
                    Password = model.User.Password
                }
            );

            TempData["SuccessMessage"] = "Your password has been changed successfully, please log in again";
            return RedirectToAction("Logout", "Login");
        }
    }
}
