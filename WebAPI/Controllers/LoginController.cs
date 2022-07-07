using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Common.Models;
using Common.Validators;
using Practice.Services;
using WebAPI.ApiModels;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        private readonly IUserService _userService;
        private readonly IValidator<IUserCredentials> _credentialsValidator;
        private readonly ILogger<LoginController> _logger;

        public LoginController(
            IAuthenticationService authService, 
            IUserService userService, 
            IValidator<IUserCredentials> credentialsValidator,
            ILogger<LoginController> logger)
        {
            _authService = authService;
            _userService = userService;
            _credentialsValidator = credentialsValidator;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<LoginResponse>> Authenticate(LoginData loginReq)
        {
            LoginResponse response = new();
            var validationResult = await _credentialsValidator.ValidateAsync(
                loginReq,
                options =>
                {
                    options.IncludeRuleSets("Username", "LoginPassword");
                }
            );
            if (!validationResult.IsValid)
            {
                response.Errors = validationResult.ToDictionarySingle();
                return BadRequest(response);
            }

            var user = _userService.FindByUserCredentials(loginReq.Username, loginReq.Password);
            if (user == null)
            {
                response.Errors = new Dictionary<string, string?>
                {
                    { "", "Username or password is incorrect" }
                };
                return StatusCode(403, response);
            }

            response.User = user;
            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            await _authService.LogOut(this.HttpContext);
            return Ok();
        }
    }
}
