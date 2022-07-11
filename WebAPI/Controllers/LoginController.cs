using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FluentValidation;
using Common.Models;
using Common.Validators;
using Common.Services;
using WebAPI.ApiModels;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ITokenBasedAuthService<string> _authService;
        private readonly IUserService _userService;
        private readonly IValidator<IUserCredentials> _credentialsValidator;
        private readonly ILogger<LoginController> _logger;

        public LoginController(
            ITokenBasedAuthService<string> authService, 
            IUserService userService, 
            IValidator<IUserCredentials> credentialsValidator,
            ILogger<LoginController> logger)
        {
            _authService = authService;
            _userService = userService;
            _credentialsValidator = credentialsValidator;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost]
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
                return StatusCode(403, response);
            }

            response.Jwt = _authService.CreateToken(
                new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username)
                });
            response.User = user;
            return Ok(response);
        }
    }
}
