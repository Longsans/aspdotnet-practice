using Microsoft.AspNetCore.Http;

namespace Common.Services
{
    public interface IClaimsBasedAuthService
    {
        Task<bool> LogIn(
            string username, 
            string password, 
            bool rememberUser, 
            IUserService userService,
            HttpContext httpContext);

        Task LogOut(HttpContext httpContext);
    }
}
