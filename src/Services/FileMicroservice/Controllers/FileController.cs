using FileMicroservice.DTOs;
using FileMicroservice.Interfaces;
using FileMicroservice.Services;
using FileMicroservice.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Text.RegularExpressions;

namespace FileMicroservice.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Produces("application/json")]
    [ApiVersion("2.0")]
    public class FileController : ControllerBase
    {
        private readonly IFileProvider _fileProvider;
        private readonly IMessagingProducer _messagingProducer;
        private readonly IRetrieveConfigHelper _retrieveConfigHelper;
        private readonly IDeleteFileHelper _deleteFileHelper;
        private readonly IRetrieveExternalSecretHelper _retrieveExternalSecretHelper;
        private readonly FileService _fileService;

        public FileController(IFileProvider fileProvider, IMessagingProducer messagingProducer, IRetrieveConfigHelper retrieveConfigHelper, IDeleteFileHelper deleteFileHelper, IRetrieveExternalSecretHelper retrieveExternalSecretHelper)
        {
            this._fileProvider = fileProvider;
            this._messagingProducer = messagingProducer;
            this._retrieveConfigHelper = retrieveConfigHelper;
            this._deleteFileHelper = deleteFileHelper;
            this._retrieveExternalSecretHelper = retrieveExternalSecretHelper;
            this._fileService = new FileService(this._fileProvider, this._messagingProducer, this._retrieveConfigHelper, this._deleteFileHelper, this._retrieveExternalSecretHelper);
        }

        /// <summary>
        /// Downloading the file from the file storage.
        /// </summary>
        /// <param name="fileName">The complete filename</param>
        /// <response code="200">File downloaded</response>
        /// <response code="400">Filename not specified</response>
        /// <response code="404">File not found</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;

            if (fileName == null)
            {
                return BadRequest(new { message = "No filename specified." });
            }

            var file = await this._fileService.RetrieveFile(fileName, userId, token);

            if (file.SingleOrDefault().Key == ResultType.FileNotFound)
            {
                return NotFound(new { message = "No file found."});
            }
            else if (file.SingleOrDefault().Key == ResultType.FileNotForUser)
            {
                return Unauthorized(new { message = "File does not belong to the user." });
            }

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return File(file.SingleOrDefault().Value, contentType, fileName);
        }

        /// <summary>
        /// Upload a file during the upload phase from the frontend.
        /// </summary>
        /// <param name="uploadFileDto">The uploaded file and its details</param>
        /// <response code="201">File successfully saved</response>
        /// <response code="400">Information not provided</response>
        /// <response code="500">Problem with saving the file</response>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadFile([FromForm] UploadFileDTO uploadFileDto)
        {
            if ( uploadFileDto.SenderId == string.Empty || uploadFileDto.ReceiverId == string.Empty || uploadFileDto.AllowedDownloads == string.Empty)
            {
                return BadRequest(new { message = "File information not provided" });
            }

            if (!int.TryParse(uploadFileDto.AllowedDownloads, out int value))
            {
                return BadRequest(new { message = "The allowedDownloads value needs to be an integer" });
            }

            else if (uploadFileDto.File == null)
            {
                return BadRequest(new { message = "File not provided" });
            }

            // Checking file type
            byte[] buffer = new byte[uploadFileDto.File.Length];
            uploadFileDto.File.OpenReadStream().Read(buffer, 0, Convert.ToInt32(uploadFileDto.File.Length));
            string content = System.Text.Encoding.UTF8.GetString(buffer);

            string fileType;
            if (uploadFileDto.File.ContentType.Contains("text") || uploadFileDto.File.ContentType.Contains("jpeg"))
            {
                return BadRequest(new { message = "File type not allowed." });
            }
            else
            {
                fileType = uploadFileDto.File.ContentType
                    .Replace("application/", "")
                    .Replace("image/", "")
                    .Replace("video/", "")
                    .Replace("font/", "")
                    .Replace("model/", "");
            }

            if (Regex.IsMatch(content, @fileType, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
            {
                await this._fileService.SaveFile(uploadFileDto);
                return Created(String.Empty, new { message = "file saved!" });
            }

            return BadRequest(new { message = "File type does not match file extension." });
        }
    }
}
