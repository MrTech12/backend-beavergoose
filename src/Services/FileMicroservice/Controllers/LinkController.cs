using FileMicroservice.Interfaces;
using FileMicroservice.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileMicroservice.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class LinkController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IFileProvider _fileProvider;
        private FileService _fileService;

        private readonly ILogger<LinkController> _logger;

        public LinkController(IConfiguration configuration, IFileProvider fileProvider, ILogger<LinkController> logger)
        {
            this._configuration = configuration;
            this._fileProvider = fileProvider;
            this._logger = logger;
        }

        /// <summary>
        /// Downloading the file from the file storage.
        /// </summary>
        /// <response code="200">File downloaded</response>
        /// <response code="404">File not found</response>
        [HttpGet("{fileName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            _fileService = new FileService(this._configuration, this._fileProvider);
            var file = await this._fileService.RetrieveFile(fileName);

            if (file == null)
            {
                return NotFound();
            }
            return File(file, "application/octet-stream", fileName);
        }

        /// <summary>
        /// Lookup the presence of a file.
        /// </summary>
        /// <response code="200">File available</response>
        /// <response code="404">File not found</response>
        [HttpGet("presence/{fileName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> FindFile(string fileName)
        {
            _fileService = new FileService(this._configuration, this._fileProvider);
            var presence = await this._fileService.CheckPresenceOfFile(fileName);

            if (presence)
            {
                return Ok("File available");
            }
            return NotFound();
        }

        /// <summary>
        /// Upload a file during the upload phase from the frontend.
        /// </summary>
        /// <response code="201">File successfully saved</response>
        /// <response code="500">Problem with saving the file</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            _fileService = new FileService(this._configuration, this._fileProvider);
            await this._fileService.SaveFile(file);
            return Created("", new { output = "file Created!" });
        }
    }
}
