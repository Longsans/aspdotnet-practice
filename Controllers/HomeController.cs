using Microsoft.AspNetCore.Mvc;
using Practice.ViewModels;

namespace Practice.Controllers
{
    public class HomeController : BaseUserCredentialsController
    {
        public IActionResult Index()
        {
            return RedirectToLogin(() => View());
        }

        public IActionResult Profile()
        {
            return RedirectToLogin(() =>
            {
                var user = UserService.FindByUsername(
                    HttpContext.Session.GetString(_sessionKeyName)!
                );
                if (user == null)
                    return View("Error");

                var model = new BaseUserViewModel
                {
                    User = user
                };
                return View(model);
            });
        }
    }
}