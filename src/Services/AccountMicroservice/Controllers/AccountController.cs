using AccountMicroservice.DTOs;
using AccountMicroservice.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace AccountMicroservice.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AccountService _accountService;

        public AccountController(UserManager<IdentityUser> userManager)
        {
            this._userManager = userManager;
            this._accountService = new AccountService(this._userManager);
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="registerDto">The information of a new user</param>
        /// <response code="200">Account successfully created</response>
        /// <response code="400">Information not provided</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(RegisterDTO registerDto)
        {
            if (!Regex.IsMatch(registerDto.Email, "^\\S+@\\S+\\.\\S+$"))
            {
                return BadRequest(new { message = "Email is not valid" });
            }

            var result = await this._accountService.CreateAccount(registerDto);
            if (!result.SingleOrDefault().Key)
            {
                return BadRequest(new { message = result.SingleOrDefault().Value });
            }
            return Ok(new { message = "User successfully created" });
        }

        /// <summary>
        /// Logging in a user
        /// </summary>
        /// <param name="loginDto">The information of an exisiting user</param>
        /// <response code="200">Successful login</response>
        /// <response code="400">user not specified</response>
        /// <response code="404">user not found</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Login(LoginDTO loginDto)
        {
            var result = await this._accountService.CheckLogin(loginDto);
            if (!result.SingleOrDefault().Key)
            {
                return BadRequest(new { message = result.SingleOrDefault().Value });
            }

            var tokenInfo = await this._accountService.GetToken(loginDto.Username);
            return Ok(new { token = tokenInfo.SingleOrDefault().Key, userId = tokenInfo.SingleOrDefault().Value });
        }
    }
}
