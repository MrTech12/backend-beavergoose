using LinkMicroservice.Interfaces;
using LinkMicroservice.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LinkMicroservice.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class LinkController : ControllerBase
    {
        private readonly ILinkRepository _linkRepository;

        public LinkController(ILinkRepository linkRepository)
        {
            this._linkRepository = linkRepository;
        }

        /// <summary>
        /// Retrieve the filename that is associated with the link address.
        /// </summary>
        /// <param name="address">The generated link</param>
        /// <response code="200">FileName located</response>
        /// <response code="404">Address not found</response>
        [HttpGet("address")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RetrieveFileName(string address)
        {
            LinkService linkService = new LinkService(this._linkRepository);
            var fileName = await linkService.RetrieveFileName(address);
            if (fileName == null)
            {
                return NotFound(new { message = "Filename not found."});
            }
            return Ok(new { filename = fileName });
        }

        /// <summary>
        /// Retrieve the links that are associated with a receiverID.
        /// </summary>
        /// <param name="receiverID">The ID of the person that received the link</param>
        /// <response code="200">FileName located</response>
        /// <response code="404">Address not found</response>
        [HttpGet("links")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RetrieveLinks(string receiverID)
        {
            LinkService linkService = new LinkService(this._linkRepository);
            var links = await linkService.RetrieveLinks(receiverID);
            if (links.Count == 0)
            {
                return NotFound(new { message = "No links found." });
            }
            return Ok(links);
        }
    }
}
