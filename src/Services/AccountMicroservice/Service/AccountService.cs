using AccountMicroservice.DTOs;
using AccountMicroservice.Helpers;
using Microsoft.AspNetCore.Identity;

namespace AccountMicroservice.Service
{
    public class AccountService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly TokenHelper tokenHelper;

        public AccountService(UserManager<IdentityUser> userManager)
        {
            this._userManager = userManager;
            tokenHelper = new TokenHelper();
        }

        public async Task<Dictionary<bool, string>> CreateAccount(RegisterDTO registerDto)
        {
            IdentityUser user = new IdentityUser()
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };

            var email = await this._userManager.FindByEmailAsync(registerDto.Email);
            if (email != null)
            {
                return new Dictionary<bool, string>() { { false, "Email already in use." } };
            }

            var username = await _userManager.FindByNameAsync(registerDto.Username);
            if (username != null)
            {
                return new Dictionary<bool, string>() { { false, "Username already in use." } };
            }

            var result = await this._userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return new Dictionary<bool, string>() { { false, result.Errors.First().Description } };
            }
            return new Dictionary<bool, string>() { { true, string.Empty } };
        }

        public async Task<Dictionary<bool, string>> CheckLogin(LoginDTO loginDto)
        {
            var user = await this._userManager.FindByNameAsync(loginDto.Username);
            if (user == null)
            {
                return new Dictionary<bool, string>() { { false, "Username not found." } };
            }

            var result = await this._userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result)
            {
                return new Dictionary<bool, string>() { { false, "Cannot login with these credentials." } };
            }

            var token = tokenHelper.CreateToken(user);
            return new Dictionary<bool, string>() { { true, token } };
        }
    }
}
