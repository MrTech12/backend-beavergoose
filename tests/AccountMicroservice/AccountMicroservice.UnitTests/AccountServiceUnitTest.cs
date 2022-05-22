using AccountMicroservice.DTOs;
using AccountMicroservice.Service;
using AccountMicroservice.UnitTests.Helpers;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AccountMicroservice.UnitTests
{
    public class AccountServiceUnitTest
    {
        private Mock<FakeUserManager> fakeUserManager;
        private AccountService _accountService;

        public AccountServiceUnitTest()
        {
            fakeUserManager = new Mock<FakeUserManager>();
            this._accountService = new AccountService(fakeUserManager.Object);
        }

        [Fact]
        public async Task EnterExistingEmail()
        {
            // Arrange
            RegisterDTO registerDto = new RegisterDTO() { Username = "Jan", Email = "jan@gmail.com" };
            IdentityUser identityUser = new IdentityUser();
            fakeUserManager.Setup(x => x.FindByEmailAsync(registerDto.Email)).ReturnsAsync(identityUser);

            // Act
            var result = await this._accountService.CreateAccount(registerDto);

            // Assert
            Assert.NotNull(result.SingleOrDefault().Value);
        }

        [Fact] 
        public async Task EnterExistingUsername()
        {
            // Arrange
            RegisterDTO registerDto = new RegisterDTO() { Username = "Jan", Email = "jan@gmail.com" };
            IdentityUser identityUser = new IdentityUser() { UserName = "Jan"};
            IdentityUser emptyUser = null;
            fakeUserManager.Setup(x => x.FindByEmailAsync(registerDto.Email)).ReturnsAsync(emptyUser);
            fakeUserManager.Setup(x => x.FindByNameAsync(registerDto.Username)).ReturnsAsync(identityUser);

            // Act
            var result = await this._accountService.CreateAccount(registerDto);

            // Assert
            Assert.NotNull(result.SingleOrDefault().Value);
        }

        [Fact]
        public async Task CreateAccountSuccessfully()
        {
            // Arrange
            RegisterDTO registerDto = new RegisterDTO() { Username = "Jan", Email = "jan@gmail.com", Password = "yyeyyDNCNE923_923@!@" };
            IdentityUser identityUser = new IdentityUser() { UserName = registerDto.Username, Email = registerDto.Email };
            IdentityUser emptyUser = null;
            fakeUserManager.Setup(x => x.FindByEmailAsync(registerDto.Email)).ReturnsAsync(emptyUser);
            fakeUserManager.Setup(x => x.FindByNameAsync(registerDto.Username)).ReturnsAsync(emptyUser);
            fakeUserManager.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await this._accountService.CreateAccount(registerDto);

            // Assert
            Assert.True(result.SingleOrDefault().Key);
        }
    }
}