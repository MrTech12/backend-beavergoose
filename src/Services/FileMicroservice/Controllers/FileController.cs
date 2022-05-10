using FileMicroservice.DTOs;
using FileMicroservice.Interfaces;
using FileMicroservice.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace FileMicroservice.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    [ApiVersion("2.0")]
    public class FileController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IFileProvider _fileProvider;
        private readonly IMessagingProducer _messagingProducer;
        private FileService _fileService;

        private readonly ILogger<FileController> _logger;

        public FileController(IConfiguration configuration, IFileProvider fileProvider, ILogger<FileController> logger, IMessagingProducer messagingProducer)
        {
            this._configuration = configuration;
            this._fileProvider = fileProvider;
            this._logger = logger;
            this._messagingProducer = messagingProducer;
            this._fileService = new FileService(this._configuration, this._fileProvider, this._messagingProducer);
        }

        /// <summary>
        /// Downloading the file from the file storage.
        /// </summary>
        /// <param name="fileName">The complete filename</param>
        /// <response code="200">File downloaded</response>
        /// <response code="400">Filename not specified</response>
        /// <response code="404">File not found</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            if (fileName == null)
            {
                return BadRequest(new { message = "No filename specified." });
            }

            var file = await this._fileService.RetrieveFile(fileName);

            if (file == null)
            {
                return NotFound(new { message = "No file found."});
            }

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return File(file, contentType, fileName);
        }

        /// <summary>
        /// Upload a file during the upload phase from the frontend.
        /// </summary>
        /// <param name="file">The uploaded file</param>
        /// <response code="201">File successfully saved</response>
        /// <response code="400">Information not provided</response>
        /// <response code="500">Problem with saving the file</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (!Request.Headers.TryGetValue("X-SenderID", out var senderID) || !Request.Headers.TryGetValue("X-ReceiverID", out var receiverID) || !Request.Headers.TryGetValue("X-AllowedDownloads", out var allowedDownloads))
            {
                return BadRequest(new { message = "File information not provided" });
            }

            if (senderID.Contains(string.Empty) || receiverID.Contains(string.Empty) || allowedDownloads.Contains(string.Empty))
            {
                return BadRequest(new { message = "File information not provided" });
            }

            if (!int.TryParse(allowedDownloads, out int value))
            {
                return BadRequest(new { message = "The allowedDownloads value needs to be an integer" });
            }

            else if (file == null)
            {
                return BadRequest(new { message = "File not provided" });
            }

            var fileDto = new FileDTO() { SenderID = senderID, ReceiverID = receiverID, AllowedDownloads = Convert.ToInt32(allowedDownloads) };


            await this._fileService.SaveFile(file, fileDto);
            return Created(String.Empty, new { message = "file saved!" });
        }
    }
}
