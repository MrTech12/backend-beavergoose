using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountMicroservice.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class AccountController : ControllerBase
    {

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult SignUp()
        {
            return Ok(new { message = "test" });
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Login(string name)
        {
            return Ok(name);
        }
    }
}
