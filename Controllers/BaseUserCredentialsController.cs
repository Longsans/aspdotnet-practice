using Microsoft.AspNetCore.Mvc;
using Practice.Services;

namespace Practice.Controllers
{
    public class BaseUserCredentialsController : Controller
    {
        private IUserService _userService;
        protected IUserService UserService => _userService ??= HttpContext.RequestServices.GetRequiredService<IUserService>();

        protected static readonly string _sessionKeyName = "username";
        protected static readonly string _usernameErrorKey = "User.Username";
        protected static readonly string _passwordErrorKey = "User.Password";

        protected IActionResult RedirectToLogin(Func<IActionResult> next)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_sessionKeyName)))
            {
                return RedirectToAction("Index", "Login", new { loginRequired = true });
            }
            return next();
        }

        protected async Task<IActionResult> RedirectToLogin(Func<Task<IActionResult>> next)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(_sessionKeyName)))
            {
                return RedirectToAction("Index", "Login", new { loginRequired = true });
            }
            return await next();
        }
    }
}
