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
        public readonly UserManager<IdentityUser> _userManager;
        private readonly Mock<FakeUserManager> fakeUserManager;
        private readonly AccountService _accountService;

        public AccountServiceUnitTest()
        {
            fakeUserManager = new Mock<FakeUserManager>();
            this._userManager = fakeUserManager.Object;
            this._accountService = new AccountService(this._userManager);
        }

        [Fact]
        public async Task EnterExistingUsername()
        {
            // Arrange
            RegisterDTO registerDto = new RegisterDTO() { Username = "Jan", Email = "jan@gmail.com" };
            fakeUserManager.Setup(x => x.FindByNameAsync(registerDto.Email)).Returns((IdentityUser user) => Task.FromResult(IdentityResult.Success));

            // Act
            var result = await this._accountService.CreateAccount(registerDto);

            // Assert
            Assert.Equal(null, result.SingleOrDefault().Value);
        }

        [Fact]
        public async Task EnterExistingEmail()
        {
            // Arrange
            RegisterDTO registerDto = new RegisterDTO() { Username = "Jan", Email = "jan@gmail.com" };
            fakeUserManager.Setup(x => x.FindByEmailAsync(registerDto.Email)).Returns((IdentityUser user) => Task.FromResult(IdentityResult.Success));

            // Act
            var result = await this._accountService.CreateAccount(registerDto);

            // Assert
            Assert.Equal(null, result.SingleOrDefault().Value);
        }

        [Fact]
        public async Task CreateAccountSuccessfully()
        {
            // Arrange
            RegisterDTO registerDto = new RegisterDTO() { Username = "Jan", Email = "jan@gmail.com", Password = "yyeyyDNCNE923_923@!@" };
            IdentityUser identityUser = new IdentityUser() { UserName = registerDto.Username, Email = registerDto.Email };
            fakeUserManager.Setup(x => x.CreateAsync(identityUser, registerDto.Password)).Returns((IdentityUser user) => Task.FromResult(IdentityResult.Success));

            // Act
            var result = await this._accountService.CreateAccount(registerDto);

            // Assert
            Assert.Equal(null, result.SingleOrDefault().Value);
        }
    }
}