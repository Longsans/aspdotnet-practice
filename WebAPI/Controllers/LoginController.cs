using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Practice.Models;
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

        public LoginController(IAuthenticationService authService, IUserService userService, IValidator<IUserCredentials> credentialsValidator)
        {
            _authService = authService;
            _userService = userService;
            _credentialsValidator = credentialsValidator;
        }

        [HttpPost]
        public async Task<ActionResult<User>> Login([FromBody] LoginData loginReq)
        {
            var validationResult = await _credentialsValidator.ValidateAsync(
                loginReq,
                options =>
                {
                    options.IncludeRuleSets("Username", "LoginPassword");
                }
            );
            if (!validationResult.IsValid)
            {
                return BadRequest("Login request data not valid");
            }

            if (!await _authService.LogIn(
                    loginReq.Username,
                    loginReq.Password,
                    loginReq.RememberUser,
                    this._userService,
                    this.HttpContext))
            {
                return StatusCode(403);
            }
            var user = _userService.FindWithContactByUsernameForDisplay(loginReq.Username);
            var response = new
            {
                user.Username,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Age,
                Contact = new
                {
                    user.Contact.Id,
                    user.Contact.Phone,
                    user.Contact.Address,
                }
            };
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
