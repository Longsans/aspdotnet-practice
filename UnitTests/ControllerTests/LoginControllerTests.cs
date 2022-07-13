using Moq;
using FluentValidation;
using Common.Services;
using Common.Models;
using Practice.ViewModels;
using Practice.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;

namespace UnitTests.ControllerTests
{
    public class LoginControllerTests
    {
        [Fact]
        public void Index_ReturnsViewResult_WhenUnauthenticated()
        {
            // arrange
            (var authServiceMock, var userCredValidatorMock) = GetAuthServiceAndCredValidator();

            var controller = new LoginController(
                userCredValidatorMock.Object, authServiceMock.Object);
            controller.AddEmptyHttpContext();
            controller.TempData = new TempDataDictionary(
                controller.HttpContext, Mock.Of<ITempDataProvider>());

            // act
            var result = controller.Index();

            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<LoginViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public void Login2_ReturnsViewResult_WhenUnauthenticated()
        {
            // arrange
            (var authServiceMock, var userValidatorMock) = GetAuthServiceAndCredValidator();
            var controller = new LoginController(
                userValidatorMock.Object, authServiceMock.Object);

            controller.AddEmptyHttpContext();

            // act
            var result = controller.Login2();

            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task LogInNoValidate_RedirectsToHome_WhenUnauthenticated()
        {
            // arrange
            (var authServiceMock, 
                var userCredValidatorMock, 
                var userServiceMock) = GetAuthServiceAndCredValidatorAndUserService();

            authServiceMock.Setup(
                x => x.LogIn(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    userServiceMock.Object,
                    It.IsAny<HttpContext>()))
                .ReturnsAsync(true);

            var controller = new LoginController(
                userCredValidatorMock.Object, authServiceMock.Object, userServiceMock.Object);
            controller.AddEmptyHttpContext();

            // act
            var result = await controller.LogInNoValidate(
                new LoginViewModel
                {
                    User = new User()
                });

            // assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }

        [Theory]
        [InlineData("Long", "`123")]
        [InlineData("asdf", "asdff")]
        public async Task Authenticate_RedirectsToHome_WhenUnauthenticated(string username, string password)
        {
            // arrange
            var authServiceMock = new Mock<IClaimsBasedAuthService>();
            var userCredValidator = new Common.Validators.UserCredentialsValidator();
            var userServiceMock = new Mock<IUserService>();

            authServiceMock.Setup(
                x => x.LogIn(
                    username,
                    password,
                    It.IsAny<bool>(),
                    userServiceMock.Object,
                    It.IsAny<HttpContext>()))
                .ReturnsAsync(
                    () => username != password);

            var controller = new LoginController(
                userCredValidator, authServiceMock.Object, userServiceMock.Object);
            controller.AddEmptyHttpContext();

            // act
            var result = await controller.Authenticate(
                new LoginViewModel
                {
                    User = new User
                    {
                        Username = username,
                        Password = password
                    }
                });

            // assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("", "`123")]
        [InlineData("1", "1")]
        public async Task Authenticate_ReturnsValidationErrorViewResult_WhenUnauthenticated_WhenUsernameValidationFails(string username, string password)
        {
            // arrange
            var authServiceMock = new Mock<IClaimsBasedAuthService>();
            var userCredValidator = new Common.Validators.UserCredentialsValidator();
            var userServiceMock = new Mock<IUserService>();

            var controller = new LoginController(
                userCredValidator, authServiceMock.Object, userServiceMock.Object);
            controller.AddEmptyHttpContext();

            // act
            var result = await controller.Authenticate(
                new LoginViewModel
                {
                    User = new User
                    {
                        Username = username,
                        Password = password
                    }
                });

            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.NotNull(viewResult.ViewData.ModelState["User.Username"]!.Errors);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("Long", "")]
        [InlineData("1", "1")]
        public async Task Authenticate_ReturnsValidationErrorViewResult_WhenUnauthenticated_WhenPasswordValidationFails(string username, string password)
        {
            // arrange
            var authServiceMock = new Mock<IClaimsBasedAuthService>();
            var userCredValidator = new Common.Validators.UserCredentialsValidator();
            var userServiceMock = new Mock<IUserService>();

            var controller = new LoginController(
                userCredValidator, authServiceMock.Object, userServiceMock.Object);
            controller.AddEmptyHttpContext();

            // act
            var result = await controller.Authenticate(
                new LoginViewModel
                {
                    User = new User
                    {
                        Username = username,
                        Password = password
                    }
                });

            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.NotNull(viewResult.ViewData.ModelState["User.Password"]!.Errors);
        }

        [Theory]
        [InlineData("qwer", "qwer")]
        [InlineData("asdf", "asdf")]
        [InlineData("zxcv", "zxcv")]
        public async Task Authenticate_ReturnsIncorrectLoginErrorViewResult_WhenUnauthenticated(string username, string password)
        {
            // arrange
            var authServiceMock = new Mock<IClaimsBasedAuthService>();
            var userCredValidator = new Common.Validators.UserCredentialsValidator();
            var userServiceMock = new Mock<IUserService>();

            authServiceMock.Setup(
                x => x.LogIn(
                    username,
                    password,
                    It.IsAny<bool>(),
                    userServiceMock.Object,
                    It.IsAny<HttpContext>()))
                .ReturnsAsync(
                    () => username != password);

            var controller = new LoginController(
                userCredValidator, authServiceMock.Object, userServiceMock.Object);
            controller.AddEmptyHttpContext();

            // act
            var result = await controller.Authenticate(
                new LoginViewModel
                {
                    User = new User
                    {
                        Username = username,
                        Password = password
                    }
                });

            // assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.False(viewResult.ViewData.ModelState.IsValid);
            Assert.Equal("Username or password is not correct", viewResult.ViewData.ModelState[""].Errors.SingleOrDefault()!.ErrorMessage);
        }

        [Fact]
        public async Task Logout_RedirectsToHome_WhenAuthenticated()
        {
            // arrange
            (var authServiceMock, var userValidatorMock) = GetAuthServiceAndCredValidator();
            authServiceMock.Setup(
                x => x.LogOut(It.IsAny<HttpContext>()));

            var controller = new LoginController(
                userValidatorMock.Object, authServiceMock.Object);

            controller.AddEmptyHttpContext();

            // act
            var result = await controller.Logout();
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Null(redirectResult.ControllerName);
        }


        private static (Mock<IClaimsBasedAuthService>, Mock<IValidator<IUserCredentials>>) GetAuthServiceAndCredValidator()
        {
            return (new Mock<IClaimsBasedAuthService>(), new Mock<IValidator<IUserCredentials>>());
        }

        private static (Mock<IClaimsBasedAuthService>, Mock<IValidator<IUserCredentials>>, Mock<IUserService>) GetAuthServiceAndCredValidatorAndUserService()
        {
            (var auth, var validator) = GetAuthServiceAndCredValidator();
            return (auth, validator, new Mock<IUserService>());
        }
    }
}
