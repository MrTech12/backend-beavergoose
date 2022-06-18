using FileMicroservice.DTOs;
using FileMicroservice.IntegrationTests.Stubs;
using FileMicroservice.Services;
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
            this.fileService = new FileService(new LocalstackFileProvider(), new StubMessagingProducer(), new StubRetrieveConfigHelper(), new StubDeleteFileHelper());
        }

        [Fact]
        public async Task SaveFileWithExtension()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverId = "12", SenderId = "24" };
            string userId = "12";
            var token = string.Empty;

            var bytes = Encoding.UTF8.GetBytes("This is a dummy text file");
            var stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "dummy.txt")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            // Act
            var newFileName = await this.fileService.SaveFile(stubFile, fileDto);

            // Assert
            var savedFile = await this.fileService.RetrieveFile(newFileName, userId, token);
            Assert.NotNull(savedFile.SingleOrDefault().Value);
        }

        [Fact]
        public async void SaveFileWithoutExtension()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverId = "12", SenderId = "24" };
            string userId = "12";
            var token = string.Empty;

            var bytes = Encoding.UTF8.GetBytes("This is a qwerty file wih no extension");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "qwerty")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            // Act
            var newFileName = await this.fileService.SaveFile(stubFile, fileDto);

            // Assert
            var savedFile = await this.fileService.RetrieveFile(newFileName, userId, token);
            Assert.NotNull(savedFile.SingleOrDefault().Value);
        }

        [Fact]
        public async void CheckIfSavedFilePresent()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverId = "12", SenderId = "24" };
            string userId = "12";
            var token = string.Empty;

            var bytes = Encoding.UTF8.GetBytes("This is a dummy text file");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "dummy.txt")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            var newFileName = await this.fileService.SaveFile(stubFile, fileDto);

            // Act
            var savedFile = await this.fileService.RetrieveFile(newFileName, userId, token);

            // Assert
            Assert.NotNull(savedFile.SingleOrDefault().Value);
        }

        [Fact]
        public async void CheckIfUnsavedFilePresent()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverId = "12", SenderId = "24" };
            string userId = "12";
            var token = string.Empty;

            var bytes = Encoding.UTF8.GetBytes("This is a azerty PDF");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "azerty.pdf")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };

            var newFileName = await this.fileService.SaveFile(stubFile, fileDto);

            // Act
            var savedFile = await this.fileService.RetrieveFile("qwerty.txt", userId, token);

            // Assert
            Assert.Null(savedFile.SingleOrDefault().Value);
        }

        [Fact]
        public async void DownloadPresentFile()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverId = "12", SenderId = "24" };
            string userId = "12";
            var token = string.Empty;

            var bytes = Encoding.UTF8.GetBytes("This is a qwerty file");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "qwerty.pdf")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };

            var newFileName = await this.fileService.SaveFile(stubFile, fileDto);

            // Act
            var fileBytes = await this.fileService.RetrieveFile(newFileName, userId, token);

            // Assert
            Assert.NotEmpty(fileBytes.SingleOrDefault().Value);
        }

        [Fact]
        public async void DownloadNonPresentFile()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverId = "12", SenderId = "24" };
            string userId = "12";
            var token = string.Empty;

            var bytes = Encoding.UTF8.GetBytes("This is a pineapple PDF");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "pineapple.pdf")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };

            var newFileName = await this.fileService.SaveFile(stubFile, fileDto);

            // Act
            var fileBytes = await this.fileService.RetrieveFile(newFileName, userId, token);

            // Assert
            Assert.Equal(23, fileBytes.SingleOrDefault().Value.Length);
        }
    }
}