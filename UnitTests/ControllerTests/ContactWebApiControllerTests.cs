using Moq;
using Common.Services;
using Common.Models;
using Common.Validators;
using WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace UnitTests.ControllerTests
{
    public class ContactWebApiControllerTests
    {
        #region Get contact
        [Fact]
        public void GetContact_ReturnsOkWithContact()
        {
            // arrange
            (var userServiceMock, var contactValidator) = GetUserServiceMockAndContactValidator();

            userServiceMock.Setup(
                x => x.FindContactByUsername(It.IsAny<string>()))
                .Returns(new Contact());
            var controller = new ContactController(userServiceMock.Object, contactValidator);

            // act
            var result = controller.GetContact("Long");

            // assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.IsType<Contact?>(okResult.Value);
        }
        #endregion


        #region Create contact
        [Theory]
        [InlineData("0901111111", "asdf", "asdf")]
        [InlineData("0222222222", "qwer", "zxcv")]
        public async Task CreateContact_ReturnsCreatedAtActionGetContact_WithRouteValueAndContactObject(
            string phone, string address, string username)
        {
            // arrange
            (var userServiceMock, var contactValidator) = GetUserServiceMockAndContactValidator();
            var contact = new Contact
            {
                Phone = phone,
                Address = address,
                UserUsername = username
            };

            ConfigureStubUserService(userServiceMock, username);
            userServiceMock.Setup(x => x.AddContact(It.IsAny<Contact>()))
                .Verifiable();
            var controller = new ContactController(userServiceMock.Object, contactValidator);

            // act
            var result = await controller.CreateContact(contact);

            // assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetContact", createdResult.ActionName);
            
            var value = Assert.IsType<Contact?>(createdResult.Value);
            Assert.NotNull(createdResult.RouteValues);
            Assert.Single(createdResult.RouteValues);
            Assert.Equal(contact.UserUsername, createdResult.RouteValues.Values.SingleOrDefault());
            Assert.Equal(contact.UserUsername, value!.UserUsername);
            userServiceMock.Verify();
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData("", "", "")]
        [InlineData("asdf", "asdf", "asdf")]
        [InlineData("0901", "asdf", "asdf")]
        public async Task CreateContact_ReturnsBadRequest_WithErrorDictionary_WhenValidationFails(
            string phone, string address, string username)
        {
            // arrange
            (var userServiceMock, var contactValidator) = GetUserServiceMockAndContactValidator();
            var controller = new ContactController(userServiceMock.Object, contactValidator);

            // act
            var result = await controller.CreateContact(
                new Contact
                {
                    Phone = phone,
                    Address = address,
                    UserUsername = username
                });

            // assert
            var badReqResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badReqResult.Value);

            var errors = Assert.IsType<Dictionary<string, string>>(badReqResult.Value);
            Assert.True(errors.Count > 0);
        }

        [Theory]
        [InlineData("0901111111", "asdf", "Long")]
        [InlineData("0901111111", "asdf", "loNg")]
        [InlineData("0901111111", "asdf", "LONG")]
        public async Task CreateContact_ReturnsConflict_WithErrorDictionary_WhenUsernameExists(
            string phone, string address, string username)
        {
            // arrange
            (var userServiceMock, var contactValidator) = GetUserServiceMockAndContactValidator();
            var contact = new Contact
            {
                Phone = phone,
                Address = address,
                UserUsername = username
            };

            ConfigureStubUserService(userServiceMock, username);
            var controller = new ContactController(userServiceMock.Object, contactValidator);

            // act
            var result = await controller.CreateContact(contact);

            // assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
            var errors = Assert.IsType<Dictionary<string, string>>(conflictResult.Value);
            var pair = Assert.Single(errors);
            Assert.Equal("username", pair.Key);
            Assert.Equal($"Contact with username {contact.UserUsername} already exists", pair.Value);
        }
        #endregion


        #region Update contact
        [Theory]
        [InlineData(null, null, null)]
        [InlineData("", "", "")]
        [InlineData("asdf", "asdf", "asdf")]
        [InlineData("0901", "asdf", "asdf")]
        public async Task UpdateContact_ReturnsBadRequest_WithErrorDictionary_WhenValidationFails(
            string phone, string address, string username)
        {
            // arrange
            (var userServiceMock, var contactValidator) = GetUserServiceMockAndContactValidator();
            var controller = new ContactController(userServiceMock.Object, contactValidator);

            // act
            var result = await controller.UpdateContact(
                username,
                new Contact
                {
                    Phone = phone,
                    Address = address,
                    UserUsername = username
                });

            // assert
            var badReqResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badReqResult.Value);

            var errors = Assert.IsType<Dictionary<string, string>>(badReqResult.Value);
            Assert.True(errors.Count > 0);
        }

        [Fact]
        public async Task UpdateContact_ReturnsBadRequest_WithErrorDictionary_WhenUsernameMismatch()
        {
            // arrange
            (var userServiceMock, var contactValidator) = GetUserServiceMockAndContactValidator();
            var controller = new ContactController(userServiceMock.Object, contactValidator);

            // act
            var result = await controller.UpdateContact(
                "Long",
                new Contact
                {
                    Phone = "0901111111",
                    Address = "asdf",
                    UserUsername = "asdf"
                });

            // assert
            var badReqResult = Assert.IsType<BadRequestObjectResult>(result);
            var errors = Assert.IsType<Dictionary<string, string>>(badReqResult.Value);
            var pair = Assert.Single(errors);
            Assert.Equal("username", pair.Key);
            Assert.Equal("Route value username must be the same as contact.UserUsername", pair.Value);
        }

        [Fact]
        public async Task UpdateContact_ReturnsNotFound_WhenUserNotFound()
        {
            // arrange
            (var userServiceMock, var contactValidator) = GetUserServiceMockAndContactValidator();
            var username = "asdf";
            ConfigureStubUserService(userServiceMock, username);
            var controller = new ContactController(userServiceMock.Object, contactValidator);

            // act
            var result = await controller.UpdateContact(
                username,
                new Contact
                {
                    Phone = "0901111111",
                    Address = "asdf",
                    UserUsername = username
                });

            // assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdateContact_ReturnsNoContent_WhenSuccess()
        {
            // arrange
            (var userServiceMock, var contactValidator) = GetUserServiceMockAndContactValidator();
            var username = "Long";
            ConfigureStubUserService(userServiceMock, username);
            var controller = new ContactController(userServiceMock.Object, contactValidator);

            // act
            var result = await controller.UpdateContact(
                username,
                new Contact
                {
                    Phone = "0922222222",
                    Address = "qwer",
                    UserUsername = username
                });

            // assert
            Assert.IsType<NoContentResult>(result);
        }
        #endregion


        #region Helper functions
        private static (Mock<IUserService>, ContactValidator) GetUserServiceMockAndContactValidator()
        {
            return (new Mock<IUserService>(), new ContactValidator());
        }

        private static void ConfigureStubUserService(Mock<IUserService> userServiceMock, string usernameParam)
        {
            userServiceMock
                .Setup(
                    x => x.FindContactByUsername(It.IsAny<string>()))
                .Returns(() =>
                {
                    Contact? contact = null;
                    if (usernameParam.ToLower() == "long")
                    {
                        contact = new Contact();
                    }
                    return contact;
                });
        }
        #endregion
    }
}
