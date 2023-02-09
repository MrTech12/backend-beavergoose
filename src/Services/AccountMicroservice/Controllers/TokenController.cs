using AccountMicroservice.DTOs;
using AccountMicroservice.Service;
using Common.Configuration.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AccountMicroservice.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class TokenController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AccountService _accountService;
        private readonly ILogger<AccountService> _accountServiceLogger;
        private readonly IConfigHelper _configHelper;

        public TokenController(UserManager<IdentityUser> userManager, ILogger<AccountService> accountServiceLogger, IConfigHelper configHelper)
        {
            this._userManager = userManager;
            this._accountServiceLogger = accountServiceLogger;
            this._configHelper = configHelper;
            this._accountService = new AccountService(this._userManager, this._configHelper, _accountServiceLogger);
        }

        /// <summary>
        /// Creating new access and refresh tokens
        /// </summary>
        /// <param name="tokenResponseDTO">The access & refresh token</param>
        /// <response code="200">Send out new tokens</response>
        /// <response code="400">Tokens not present</response>
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Refresh(TokenResponseDTO tokenResponseDTO)
        {
            if (tokenResponseDTO == null)
            {
                return BadRequest(new { message = "The tokens are not present." });
            }
            var response = await this._accountService.CreateNewTokens(tokenResponseDTO.AccessToken);

            return Ok(new { AccessToken = response.AccessToken, RefreshToken = response.RefreshToken });
        }
    }
}
