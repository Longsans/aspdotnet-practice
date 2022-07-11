using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Practice.ViewModels;
using Common.Models;
using Practice.Validators;

namespace Practice.Controllers
{
    public class AccountSettingsController : BaseUserCredentialsController
    {
        private readonly IValidator<IUserInfo> _userInfoValidator;
        private readonly IValidator<AccountSettingsViewModel> _accountSettingsValidator;
        private readonly IValidator<Contact> _contactValidator;

        public AccountSettingsController(
            IValidator<IUserInfo> userInfoValidator, 
            IValidator<AccountSettingsViewModel> accountSettingsValidator,
            IValidator<Contact> contactValidator)
        {
            _userInfoValidator = userInfoValidator;
            _accountSettingsValidator = accountSettingsValidator;
            _contactValidator = contactValidator;
        }

        public IActionResult Index()
        {
            var user = UserService.FindWithContactByUsernameForDisplay(HttpContext.User.Identity.Name);
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
            var validateResult = await _userInfoValidator.ValidateAsync(
                model.User, 
                options =>
                {
                    options.IncludeRuleSets("UserInfo");
                }
            );

            model.User = UserService.FindWithContactByUsernameForDisplay(model.User.Username);
            if (!validateResult.IsValid)
            {
                validateResult.AddToModelState(ModelState, "User");
                return View(nameof(Index), model);
            }

            await UserService.UpdateUserInfo(new User(model.User));
            model.SuccessMessage = "Update personal info success!";
            return View(nameof(Index), model);
        }

        public async Task<IActionResult> SaveContact(AccountSettingsViewModel model)
        {
            ModelState.Clear();
            var validateResult = await _contactValidator.ValidateAsync(model.User.Contact);
            if (!validateResult.IsValid)
            {
                validateResult.AddToModelState(ModelState, "User.Contact");
                return View(nameof(Index), model);
            }

            model.User.Contact.UserUsername = model.User.Username;
            var contact = UserService.FindContactByUsername(model.User.Contact.UserUsername);
            if (contact != null)
            {
                await UserService.UpdateContact(model.User.Contact);
            }
            else
            {
                await UserService.AddContact(model.User.Contact);
            }

            model.ContactSuccessMessage = "Save contact success!";
            return View(nameof(Index), model);
        }

        public async Task<IActionResult> SavePassword(AccountSettingsViewModel model)
        {
            ModelState.Clear();
            var validateResult = await _accountSettingsValidator.ValidateAsync(
                model,
                options =>
                {
                    options.IncludeRulesNotInRuleSet();
                }
            );
            if (!validateResult.IsValid)
            {
                validateResult.AddToModelStatePrefixed(ModelState, new Dictionary<string, string>
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
