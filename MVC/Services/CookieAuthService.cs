using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Common.Services;

namespace Practice.Services
{
    public class CookieAuthService : IClaimsBasedAuthService
    {
        public async Task<bool> LogIn(
            string username, 
            string password, 
            bool rememberUser, 
            IUserService userService,
            HttpContext httpContext)
        {
            var user = userService.FindByUserCredentials(username, password);
            if (user == null)
            {
                return false;
            }

            var claimsIdentity = new ClaimsIdentity(
                new Claim[]
                {
                    new Claim(ClaimTypes.Name, username)
                },
                CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);
            var authProps = new AuthenticationProperties
            {
                IsPersistent = true
            };
            if (!rememberUser)
            {
                authProps.IsPersistent = false;
                authProps.ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1);
                authProps.AllowRefresh = false;
            }

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProps
            );

            return true;
        }

        public async Task LogOut(HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
