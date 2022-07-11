using Microsoft.AspNetCore.Mvc;
using Practice.ViewModels;

namespace Practice.Controllers
{
    public class HomeController : BaseUserCredentialsController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Profile()
        {
            var user = UserService.FindByUsername(HttpContext.User.Identity!.Name!);
            if (user == null)
                return View("Error");

            var model = new BaseUserViewModel
            {
                User = user
            };
            return View(model);
        }
    }
}