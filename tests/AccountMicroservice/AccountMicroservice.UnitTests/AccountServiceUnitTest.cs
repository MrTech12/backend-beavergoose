using AccountMicroservice.DTOs;
using AccountMicroservice.Helpers;
using AccountMicroservice.Service;
using AccountMicroservice.UnitTests.Helpers;
using AccountMicroservice.UnitTests.Stubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AccountMicroservice.UnitTests
{
    public class AccountServiceUnitTest
    {
        private Mock<FakeUserManager> fakeUserManager;
        private AccountService _accountService;
        private Mock<TokenHelper> fakeTokenHelper;

        public AccountServiceUnitTest()
        {
            fakeUserManager = new Mock<FakeUserManager>();
            fakeTokenHelper = new Mock<TokenHelper>();

            var loggerMock = new Mock<ILogger<AccountService>>();
            ILogger<AccountService> accountServiceLogger = loggerMock.Object;

            this._accountService = new AccountService(fakeUserManager.Object, new StubConfigHelper(), accountServiceLogger);
        }

        [Fact]
        public async Task AccountCreation_ExistingEmail()
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
        public async Task AccountCreation_ExistingUsername()
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
        public async Task AccountCreation_CreateSuccessfully()
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

        [Fact]
        public async Task Login_UnknownUsername()
        {
            // Arrange
            LoginDTO loginDto = new LoginDTO() { Username = "Ben", Password = "yyeyyDNCNE923_923@!@" };
            IdentityUser emptyUser = null;
            fakeUserManager.Setup(x => x.FindByNameAsync(loginDto.Username)).ReturnsAsync(emptyUser);

            // Act
            var result = await this._accountService.CheckLogin(loginDto);

            // Assert
            Assert.False(result.SingleOrDefault().Key);
        }

        [Fact]
        public async Task Login_InvalidPassword()
        {
            // Arrange
            LoginDTO loginDto = new LoginDTO() { Username = "Ben", Password = "yyeyyDNCNE923_923@!@" };
            IdentityUser emptyUser = null;
            fakeUserManager.Setup(x => x.FindByNameAsync(loginDto.Username)).ReturnsAsync(emptyUser);
            fakeUserManager.Setup(x => x.CheckPasswordAsync(It.IsAny<IdentityUser>(), loginDto.Password)).ReturnsAsync(false);

            // Act
            var result = await this._accountService.CheckLogin(loginDto);

            // Assert
            Assert.False(result.SingleOrDefault().Key);
        }
    }
}