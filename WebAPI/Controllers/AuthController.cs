using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FluentValidation;
using Common.Models;
using Common.Validators;
using Common.Services;
using WebAPI.ApiModels;
using WebAPI.Services;
using WebAPI.Utilities;

namespace WebAPI.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenBasedAuthService<string> _authService;
        private readonly IUserService _userService;
        private readonly IValidator<IUserCredentials> _credentialsValidator;
        private readonly ILogger<AuthController> _logger;

        private const string usernameClaimKey = "username";

        public AuthController(
            ITokenBasedAuthService<string> authService, 
            IUserService userService, 
            IValidator<IUserCredentials> credentialsValidator,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _userService = userService;
            _credentialsValidator = credentialsValidator;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/login")]
        public async Task<ActionResult<LoginResponse>> Authenticate(LoginData loginReq)
        {
            LoginResponse response = new();
            var validationResult = await _credentialsValidator.ValidateAsync(
                loginReq,
                options =>
                {
                    options.IncludeRuleSets("Username", "LoginPassword");
                });
            if (!validationResult.IsValid)
            {
                response.Errors = validationResult.ToErrorDictionary();
                return BadRequest(response);
            }

            var user = _userService.FindByUserCredentials(
                loginReq.Username, loginReq.Password);
            if (user == null)
            {
                response.Errors = new Dictionary<string, string?>
                {
                    { "", "Username or password is incorrect" }
                };
                return StatusCode(403, response); // Forbidden
            }

            response.AccessToken = _authService.CreateAccessToken(
                new Claim[]
                {
                    new Claim(usernameClaimKey, user.Username)
                });
            response.User = new
            {
                user.Username,
                user.FirstName,
                user.LastName,
            };
            var cookieOptions = new CookieOptions 
            { 
                HttpOnly = true, 
                SameSite = SameSiteMode.None, 
                Secure = true
            };
            if (loginReq.RememberUser)
            {
                cookieOptions.Expires = DateTime.Now.AddMonths(3);
            }
            this.Response.Cookies.Append(
                Statics.RefreshTokenCookieName, 
                _authService.CreateRefreshToken(
                    new Claim[] { new Claim(usernameClaimKey, user.Username) }
                ), cookieOptions);
            return Ok(response);
        }

        [Authorize(Policy = Statics.RefreshTokenPolicy)]
        [HttpPost]
        [Route("api/refresh-token")]
        public ActionResult<string?> RefreshToken()
        {
            string accessToken = _authService.CreateAccessToken(
                new Claim[]
                {
                    new Claim(usernameClaimKey, User.FindFirstValue(usernameClaimKey))
                });
            return Ok(accessToken);
        }

        [HttpPost]
        [Route("api/logout")]
        public ActionResult LogOut()
        {
            this.Response.Cookies.Delete(Statics.RefreshTokenCookieName);
            return NoContent();
        }
    }
}
