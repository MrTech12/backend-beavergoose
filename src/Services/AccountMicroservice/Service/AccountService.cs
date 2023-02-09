using AccountMicroservice.DTOs;
using AccountMicroservice.Helpers;
using Common.Configuration.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AccountMicroservice.Service
{
    public class AccountService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly TokenHelper tokenHelper;
        private readonly ILogger _logger;
        private readonly IConfigHelper _configHelper;

        public AccountService(UserManager<IdentityUser> userManager, IConfigHelper configHelper, ILogger<AccountService> logger)
        {
            this._logger = logger;
            this._userManager = userManager;
            this._configHelper = configHelper;
            tokenHelper = new TokenHelper(this._configHelper);
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

            var userDto = new UserDTO() { UserId = user.Id, UserName = user.UserName };
            var userId = user.Id.ToString();

            this._logger.LogInformation("Creating JWT access & refresh tokens for login evidence");
            var tokenResponseDto = new TokenResponseDTO();
            tokenResponseDto.AccessToken = tokenHelper.CreateAccessToken(userDto);
            tokenResponseDto.RefreshToken = tokenHelper.CreateRefreshToken();

            return new Dictionary<string, TokenResponseDTO>() { { userId, tokenResponseDto } };
        }

        public async Task<TokenResponseDTO> CreateNewTokens(string accessToken)
        {
            var principal = this.tokenHelper.GetPrincipalFromExpiredAccessToken(accessToken);
            var userId = principal.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            
            this._logger.LogInformation("Finding user in db based on id");
            var user = await this._userManager.FindByIdAsync(userId);
            var userDto = new UserDTO() { UserId = user.Id, UserName = user.UserName };

            this._logger.LogInformation("Creating new JWT access & refresh tokens for login evidence");
            var newAccessToken = tokenHelper.CreateAccessToken(userDto);
            var newRefreshToken = tokenHelper.CreateRefreshToken();

            return new TokenResponseDTO() { AccessToken = newAccessToken, RefreshToken = newRefreshToken };
        }

        public List<UserDTO> GetAllUsers()
        {
            this._logger.LogInformation("Retrieving all user in db.");
            var userEntries = this._userManager.Users.Select(x => new { x.Id, x.UserName }).ToList();
            
            var userDtos = new List<UserDTO>();
            foreach (var userEntry in userEntries)
            {
                userDtos.Add(new UserDTO() { UserId = userEntry.Id, UserName = userEntry.UserName });
            }

            return userDtos;
        }
    }
}
