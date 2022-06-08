using DeleteFileApp.DTOs;
using DeleteFileApp.Interfaces;
using DeleteFileApp.Services;
using DeleteFileApp.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeleteFileApp.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class DeleteFileController : ControllerBase
    {
        private readonly ILogger<DeleteFileController> _logger;
        private readonly IFileProvider _fileProvider;
        private readonly IRetrieveConfigHelper _retrieveConfigHelper;
        private DeleteFileService _deleteFileService;

        public DeleteFileController(ILogger<DeleteFileController> logger, IFileProvider fileProvider, IRetrieveConfigHelper retrieveConfigHelper)
        {
            _logger = logger;
            _fileProvider = fileProvider;
            _retrieveConfigHelper = retrieveConfigHelper;
            _deleteFileService = new DeleteFileService(_fileProvider, _retrieveConfigHelper);
        }

        /// <summary>
        /// Delete a file from the file storage.
        /// </summary>
        /// <param name="fileName">The complete filename</param>
        /// <response code="200">File deleted successfully</response>
        /// <response code="401">File does not belong to the user</response>
        /// <response code="404">File not found</response>
        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;

            var deleteFileDTO = new DeleteFileDTO() { UserId = userId, FileName = fileName };
            var result = await _deleteFileService.RemoveFile(deleteFileDTO);

            if (result == ResultType.FileNotFound)
            {
                return NotFound(new { message = "File does not exist" });
            }
            else if (result == ResultType.FileNotForUser)
            {
                return Unauthorized(new {message = "File does not belong to the user." });
            }
            return Ok(new { message = "File deleted." });
        }
    }
}
