using Microsoft.AspNetCore.Mvc;
using Common.Services;

namespace Practice.Controllers
{
    public class BaseUserCredentialsController : Controller
    {
        private IUserService _userService;
        protected IUserService UserService => _userService ??= HttpContext.RequestServices.GetRequiredService<IUserService>();

        protected static readonly string _sessionKeyName = "username";
        protected static readonly string _usernameErrorKey = "User.Username";
        protected static readonly string _passwordErrorKey = "User.Password";

        protected IActionResult RedirectToHomeIfAuthenticated(Func<IActionResult> next)
        {
            if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return next();
        }

        protected async Task<IActionResult> RedirectToHomeIfAuthenticated(Func<Task<IActionResult>> next)
        {
            if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return await next();
        }
    }
}
