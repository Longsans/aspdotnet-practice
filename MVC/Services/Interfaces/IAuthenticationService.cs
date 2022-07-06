namespace Practice.Services
{
    public interface IAuthenticationService
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
