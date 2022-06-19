using FileMicroservice.DTOs;
using FileMicroservice.IntegrationTests.Stubs;
using FileMicroservice.Services;
using FileMicroservice.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FileMicroservice.IntegrationTests
{
    public class FileServiceIntegrationTest
    {
        private FileService fileService;

        public FileServiceIntegrationTest()
        {
            this.fileService = new FileService(new LocalstackFileProvider(), new StubMessagingProducer(), new StubRetrieveConfigHelper(), new StubDeleteFileHelper(), new StubRetrieveExternalSecretHelper());
        }

        [Fact]
        public async Task SaveFileWithExtension()
        {
            // Arrange
            var UploadFileDto = new UploadFileDTO() { ReceiverId = "12", SenderId = "24", AllowedDownloads = "1" };
            string userId = "12";
            var token = string.Empty;

            var bytes = Encoding.UTF8.GetBytes("This is a dummy text file");
            var stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "dummy.txt")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            UploadFileDto.File = stubFile;

            // Act
            var newFileName = await this.fileService.SaveFile(UploadFileDto);

            // Assert
            var savedFile = await this.fileService.RetrieveFile(newFileName, userId, token);
            Assert.NotNull(savedFile.SingleOrDefault().Value);
        }

        [Fact]
        public async void SaveFileWithoutExtension()
        {
            // Arrange
            var UploadFileDto = new UploadFileDTO() { ReceiverId = "12", SenderId = "24", AllowedDownloads = "1" };
            string userId = "12";
            var token = string.Empty;

            var bytes = Encoding.UTF8.GetBytes("This is a qwerty file wih no extension");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "qwerty")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            UploadFileDto.File = stubFile;

            // Act
            var newFileName = await this.fileService.SaveFile(UploadFileDto);

            // Assert
            var savedFile = await this.fileService.RetrieveFile(newFileName, userId, token);
            Assert.NotNull(savedFile.SingleOrDefault().Value);
        }

        [Fact]
        public async void CheckIfSavedFilePresent()
        {
            // Arrange
            var UploadFileDto = new UploadFileDTO() { ReceiverId = "12", SenderId = "24", AllowedDownloads = "1" };
            string userId = "12";
            var token = string.Empty;

            var bytes = Encoding.UTF8.GetBytes("This is a dummy text file");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "dummy.txt")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            UploadFileDto.File = stubFile;

            var newFileName = await this.fileService.SaveFile(UploadFileDto);

            // Act
            var savedFile = await this.fileService.RetrieveFile(newFileName, userId, token);

            // Assert
            Assert.NotNull(savedFile.SingleOrDefault().Value);
        }

        [Fact]
        public async void CheckIfUnsavedFilePresent()
        {
            // Arrange
            var UploadFileDto = new UploadFileDTO() { ReceiverId = "12", SenderId = "24", AllowedDownloads = "1" };
            string userId = "12";
            var token = string.Empty;

            var bytes = Encoding.UTF8.GetBytes("This is a azerty PDF");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "azerty.pdf")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };

            UploadFileDto.File = stubFile;

            var newFileName = await this.fileService.SaveFile(UploadFileDto);

            // Act
            var savedFile = await this.fileService.RetrieveFile("qwerty.txt", userId, token);

            // Assert
            Assert.Null(savedFile.SingleOrDefault().Value);
        }

        [Fact]
        public async void DownloadPresentFile()
        {
            // Arrange
            var UploadFileDto = new UploadFileDTO() { ReceiverId = "12", SenderId = "24", AllowedDownloads = "1" };
            string userId = "12";
            var token = string.Empty;

            var bytes = Encoding.UTF8.GetBytes("This is a qwerty file");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "qwerty.pdf")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };

            UploadFileDto.File = stubFile;

            var newFileName = await this.fileService.SaveFile(UploadFileDto);

            // Act
            var fileBytes = await this.fileService.RetrieveFile(newFileName, userId, token);

            // Assert
            Assert.NotEmpty(fileBytes.SingleOrDefault().Value);
        }

        [Fact]
        public async void DownloadNonPresentFile()
        {
            // Arrange
            var UploadFileDto = new UploadFileDTO() { ReceiverId = "12", SenderId = "24", AllowedDownloads = "1" };
            string userId = "12";
            var token = string.Empty;

            var bytes = Encoding.UTF8.GetBytes("This is a pineapple PDF");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "pineapple.pdf")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };

            UploadFileDto.File = stubFile;

            var newFileName = await this.fileService.SaveFile(UploadFileDto);

            // Act
            var fileBytes = await this.fileService.RetrieveFile("Orange.pdf", userId, token);

            // Assert
            Assert.Equal(ResultType.FileNotFound, fileBytes.SingleOrDefault().Key);
        }

        [Fact]
        public async void DownloadPresentFileFromAnotherUser()
        {
            // Arrange
            var uploadFileDto = new UploadFileDTO() { ReceiverId = "12", SenderId = "24", AllowedDownloads = "1" };
            var userId = "35";
            var token = string.Empty;

            var bytes = Encoding.UTF8.GetBytes("This is a Mango PDF");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "Mango.pdf")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };

            uploadFileDto.File = stubFile;

            var newFileName = await this.fileService.SaveFile(uploadFileDto);

            // Act
            var fileBytes = await this.fileService.RetrieveFile(newFileName, userId, token);

            // Assert
            File.Delete(Path.GetTempPath() + newFileName);
            Assert.Equal(ResultType.FileNotForUser, fileBytes.SingleOrDefault().Key);
        }
    }
}