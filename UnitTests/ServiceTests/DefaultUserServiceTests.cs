using Moq;
using Common.Services;
using Common.Models;
using Common.Utilities;
using Practice.Services;
using Practice.Data;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.ServiceTests
{
    public class DefaultUserServiceTests
    {
        private readonly DbContextOptions<WebAppContext> _dbContextOptions;

        public DefaultUserServiceTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<WebAppContext>()
                .UseInMemoryDatabase("DefaultUserServiceTest")
                .Options;
        }

        #region FindByUsername
        [Theory]
        [InlineData("qwer")]
        [InlineData("asdf")]
        public void FindByUsername_ReturnsUser_WhenFound(string username)
        {
            // arrange
            var userService = GetUserService();

            // act
            var result = userService.FindByUsername(username);

            // assert
            Assert.NotNull(result);
            Assert.Equal(username, result.Username);
        }

        [Fact]
        public void FindByUsername_ReturnsNull_WhenNotFound()
        {
            // arrange
            var userService = GetUserService();
            var username = "zxcv";

            // act
            var result = userService.FindByUsername(username);

            // assert
            Assert.Null(result);
        }
        #endregion

        #region UpdateUserInfo
        [Fact]
        public async Task UpdateUserInfo_ThrowsInvalidOperationException_WhenUserNotFound()
        {
            // arrange
            var userService = GetUserService();
            var user = new User
            {
                FirstName = "jimmy",
                LastName = "james",
                Email = "jim@gmail.com",
                Age = 24,
                Username = "zxcv"
            };

            // act & assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await userService.UpdateUserInfo(user));
        }

        [Fact]
        public async Task UpdateUserInfo_SavesChanges_WhenSuccess()
        {
            // arrange
            var userService = GetUserService();
            var user = new User
            {
                FirstName = "jimmy",
                LastName = "james",
                Email = "jim@gmail.com",
                Age = 24,
                Username = "asdf"
            };

            // act
            await userService.UpdateUserInfo(user);

            // assert
            var updated = userService.FindByUsername(user.Username); // this makes this test dependent on the correctness of FindByUsername
            Assert.Equal(user.FirstName, updated.FirstName);
            Assert.Equal(user.LastName, updated.LastName);
            Assert.Equal(user.Email, updated.Email);
            Assert.Equal(user.Age, updated.Age);
        }
        #endregion

        #region Helper methods
        private DefaultUserService GetUserService()
        {
            return new DefaultUserService(CreateDb());
        }

        private WebAppContext CreateDb()
        {
            var context = new WebAppContext(_dbContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Users.AddRange(
                new User
                {
                    Username = "qwer",
                    Password = Encrypter.EncryptSHA256("qwerr"),
                    FirstName = "qwer",
                    LastName = "qwer",
                    Email = "qwer@gmail.com",
                    Age = 30,
                    Contact = new Contact
                    {
                        Phone = "0901111111",
                        Address = "asdf",
                        UserUsername = "qwer"
                    }
                },
                new User
                {
                    Username = "asdf",
                    Password = Encrypter.EncryptSHA256("asdff"),
                    FirstName = "asdf",
                    LastName = "asdf",
                    Email = "asdf@gmail.com",
                    Age = 12,
                    Contact = new Contact
                    {
                        Phone = "0922222222",
                        Address = "asdf",
                        UserUsername = "asdf"
                    }
                });
            context.SaveChanges();
            return context;
        }
        #endregion
    }
}