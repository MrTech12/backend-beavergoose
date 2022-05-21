using AccountMicroservice.DTOs;
using Microsoft.AspNetCore.Identity;

namespace AccountMicroservice.Service
{
    public class AccountService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AccountService(UserManager<IdentityUser> userManager)
        {
            this._userManager = userManager;
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

        public string CheckLogin()
        {
            return "";
        }
    }
}
