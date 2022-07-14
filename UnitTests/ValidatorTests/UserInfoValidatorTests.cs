using Common.Validators;
using Common.Models;
using FluentValidation.TestHelper;

namespace UnitTests.ValidatorTests
{
    public class UserInfoValidatorTests
    {
        private UserInfoValidator Validator { get; }
        private static readonly string _ruleSet = "UserInfo";

        public UserInfoValidatorTests()
        {
            Validator = new UserInfoValidator();
        }

        #region FirstName
        [Fact]
        public async Task FirstName_ShouldNotBeEmpty()
        {
            // arrange
            var user = GetDefaultUser();
            user.FirstName = null;

            // act
            var result = await Validator.TestValidateAsync(
                user,
                options =>
                {
                    options.IncludeRuleSets(_ruleSet);
                });

            // assert
            result.ShouldHaveValidationErrorFor(x => x.FirstName)
                .WithErrorCode(ValidationErrorCodes.NotEmpty);
        }

        [Theory]
        [InlineData("asdfasdfasdfasdfasdfasdfasdf")]
        public async Task FirstName_ShouldNotBeLessThan_1_OrMoreThan_20_Chars(string firstName)
        {
            // arrange
            var user = GetDefaultUser();
            user.FirstName = firstName;

            // act
            var result = await Validator.TestValidateAsync(
                user,
                options =>
                {
                    options.IncludeRuleSets(_ruleSet);
                });

            // assert
            result.ShouldHaveValidationErrorFor(x => x.FirstName)
                .WithErrorCode(ValidationErrorCodes.Length);
        }

        [Fact]
        public async Task FirstName_ShouldBeNotEmpty_And_Length_1_To_20_Chars()
        {
            // arrange
            var user = GetDefaultUser();

            // act
            var result = await Validator.TestValidateAsync(
                user,
                options =>
                {
                    options.IncludeRuleSets(_ruleSet);
                });

            // assert
            result.ShouldNotHaveValidationErrorFor(x => x.FirstName);
        }
        #endregion

        #region LastName
        [Fact]
        public async Task LastName_ShouldNotBeEmpty()
        {
            // arrange
            var user = GetDefaultUser();
            user.LastName = null;

            // act
            var result = await Validator.TestValidateAsync(
                user,
                options =>
                {
                    options.IncludeRuleSets(_ruleSet);
                });

            // assert
            result.ShouldHaveValidationErrorFor(x => x.LastName)
                .WithErrorCode(ValidationErrorCodes.NotEmpty);
        }

        [Theory]
        [InlineData("asdfasdfasdfasdfasdfasdfasdf")]
        public async Task LastName_ShouldNotBeLessThan_1_OrMoreThan_20_Chars(string lastName)
        {
            // arrange
            var user = GetDefaultUser();
            user.LastName = lastName;

            // act
            var result = await Validator.TestValidateAsync(
                user,
                options =>
                {
                    options.IncludeRuleSets(_ruleSet);
                });

            // assert
            result.ShouldHaveValidationErrorFor(x => x.LastName)
                .WithErrorCode(ValidationErrorCodes.Length);
        }

        [Fact]
        public async Task LastName_ShouldBeNotEmpty_And_Length_1_To_20_Chars()
        {
            // arrange
            var user = GetDefaultUser();

            // act
            var result = await Validator.TestValidateAsync(
                user,
                options =>
                {
                    options.IncludeRuleSets(_ruleSet);
                });

            // assert
            result.ShouldNotHaveValidationErrorFor(x => x.LastName);
        }
        #endregion

        #region Email
        [Fact]
        public async Task Email_ShouldNotBeEmpty()
        {
            // arrange
            var user = GetDefaultUser();
            user.Email = null;

            // act
            var result = await Validator.TestValidateAsync(
                user,
                options =>
                {
                    options.IncludeRuleSets(_ruleSet);
                });

            // assert
            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorCode(ValidationErrorCodes.NotEmpty);
        }

        [Fact]
        public async Task Email_ShouldNotBeMoreThan_30_Chars()
        {
            // arrange
            var user = GetDefaultUser();
            user.Email = "asdfasdfasdfasdfasdfasdfasdfasd";

            // act
            var result = await Validator.TestValidateAsync(
                user,
                options =>
                {
                    options.IncludeRuleSets(_ruleSet);
                });

            // assert
            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorCode(ValidationErrorCodes.MaximumLength);
        }

        [Theory]
        [InlineData("asdf")]
        [InlineData("asdf@")]
        [InlineData("asdf@@asdf")]
        [InlineData("asdfasdf.")]
        public async Task Email_ShouldNotBeWrongEmailFormat(string email)
        {
            // arrange
            var user = GetDefaultUser();
            user.Email = email;

            // act
            var result = await Validator.TestValidateAsync(
                user,
                options =>
                {
                    options.IncludeRuleSets(_ruleSet);
                });

            // assert
            result.ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorCode(ValidationErrorCodes.Email);
        }

        [Fact]
        public async Task Email_ShouldBeNotEmpty_And_MaxLength_30_Chars_And_CorrectEmailFormat()
        {
            // arrange
            var user = GetDefaultUser();

            // act
            var result = await Validator.TestValidateAsync(
                user,
                options =>
                {
                    options.IncludeRuleSets(_ruleSet);
                });

            // assert
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }
        #endregion

        #region Age
        [Fact]
        public async Task Age_ShouldNotBeNull()
        {
            // arrange
            var user = GetDefaultUser();
            user.Age = null;

            // act
            var result = await Validator.TestValidateAsync(
                user,
                options =>
                {
                    options.IncludeRuleSets(_ruleSet);
                });

            // assert
            result.ShouldHaveValidationErrorFor(x => x.Age)
                .WithErrorCode(ValidationErrorCodes.NotNull);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(201)]
        public async Task Age_ShouldNotBeLessThan_0_OrMoreThan_200(int age)
        {
            // arrange
            var user = GetDefaultUser();
            user.Age = age;

            // act
            var result = await Validator.TestValidateAsync(
                user,
                options =>
                {
                    options.IncludeRuleSets(_ruleSet);
                });

            // assert
            result.ShouldHaveValidationErrorFor(x => x.Age)
                .WithErrorCode(ValidationErrorCodes.InclusiveBetween);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(21)]
        [InlineData(199)]
        [InlineData(200)]
        public async Task Age_ShouldBeNotNull_And_InclusiveBetween_0_And_200(int age)
        {
            // arrange
            var user = GetDefaultUser();
            user.Age = age;

            // act
            var result = await Validator.TestValidateAsync(
                user,
                options =>
                {
                    options.IncludeRuleSets(_ruleSet);
                });

            // assert
            result.ShouldNotHaveValidationErrorFor(x => x.Age);
        }
        #endregion

        private static User GetDefaultUser()
        {
            return new User
            {
                FirstName = "Long",
                LastName = "Do",
                Email = "long@gmail.com",
                Age = 21
            };
        }
    }
}
