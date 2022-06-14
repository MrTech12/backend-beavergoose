using AccountMicroservice.DTOs;
using AccountMicroservice.Helpers;
using Microsoft.AspNetCore.Identity;

namespace AccountMicroservice.Service
{
    public class AccountService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly TokenHelper tokenHelper;
        private readonly ILogger _logger;

        public AccountService(UserManager<IdentityUser> userManager, ILogger<AccountService> logger)
        {
            this._userManager = userManager;
            tokenHelper = new TokenHelper();
            this._logger = logger;
        }

        public async Task<Dictionary<bool, string>> CreateAccount(RegisterDTO registerDto)
        {
            IdentityUser user = new IdentityUser()
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };

            this._logger.LogInformation("Checking entered mail during account creation");
            var email = await this._userManager.FindByEmailAsync(registerDto.Email);
            if (email != null)
            {
                return new Dictionary<bool, string>() { { false, "Email already in use." } };
            }

            this._logger.LogInformation("Checking entered username during account creation");
            var username = await _userManager.FindByNameAsync(registerDto.Username);
            if (username != null)
            {
                return new Dictionary<bool, string>() { { false, "Username already in use." } };
            }

            this._logger.LogInformation("Creating a new account");
            var result = await this._userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return new Dictionary<bool, string>() { { false, result.Errors.First().Description } };
            }
            return new Dictionary<bool, string>() { { true, string.Empty } };
        }

        public async Task<Dictionary<bool, string>> CheckLogin(LoginDTO loginDto)
        {
            this._logger.LogInformation("Checking entered username during login checks");
            var user = await this._userManager.FindByNameAsync(loginDto.Username);
            if (user == null)
            {
                return new Dictionary<bool, string>() { { false, "Username not found." } };
            }

            this._logger.LogInformation("Checking if username and password of login checks matches");
            var result = await this._userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result)
            {
                return new Dictionary<bool, string>() { { false, "Cannot login with these credentials." } };
            }

            return new Dictionary<bool, string>() { { true, string.Empty } };
        }

        public async Task<Dictionary<string, TokenResponseDTO>> GetTokens(string username)
        {
            this._logger.LogInformation("Finding entered username for token creation");
            var user = await this._userManager.FindByNameAsync(username);

            var userId = user.Id.ToString();
            this._logger.LogInformation("Creating JWT access & refresh tokens for login evidence");
            
            var tokenResponseDTO = new TokenResponseDTO();
            tokenResponseDTO.AccessToken = tokenHelper.CreateAccessToken(user);
            tokenResponseDTO.RefreshToken = tokenHelper.CreateRefreshToken();
            return new Dictionary<string, TokenResponseDTO>() { { userId, tokenResponseDTO } };
        }

        public async Task<TokenResponseDTO> CreateNewTokens(string accessToken)
        {
            var principal = this.tokenHelper.GetPrincipalFromExpiredAccessToken(accessToken);
            var userId = principal.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            
            this._logger.LogInformation("Finding user in db based on id");
            var user = await this._userManager.FindByIdAsync(userId);

            this._logger.LogInformation("Creating new JWT access & refresh tokens for login evidence");
            var newAccessToken = tokenHelper.CreateAccessToken(user);
            var newRefreshToken = tokenHelper.CreateRefreshToken();

            return new TokenResponseDTO() { AccessToken = newAccessToken, RefreshToken = newRefreshToken };
        }
    }
}
