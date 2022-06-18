using FileMicroservice.DTOs;
using FileMicroservice.Services;
using FileMicroservice.UnitTests.Stubs;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace FileMicroservice.UnitTests
{
    public class FileServiceUnitTest
    {
        private FileService fileService;

        public FileServiceUnitTest()
        {
            this.fileService = new FileService(new StubFileProvider(), new StubMessagingProducer(), new StubRetrieveConfigHelper(), new StubDeleteFileHelper());
        }

        [Fact]
        public async void SaveFileWithExtension()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverId = "12", SenderId = "24" };
            var userId = "12";
            var token = string.Empty;

            string fileName = "dummy.txt";
            var bytes = Encoding.UTF8.GetBytes("This is a dummy text file");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);

            // Act
            var newFileName = await this.fileService.SaveFile(stubFile, fileDto);

            // Assert
            var savedFile = await this.fileService.RetrieveFile(newFileName, userId ,token);
            Assert.NotNull(savedFile.Values);
        }

        [Fact]
        public async void SaveFileWithoutExtension()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverId = "12", SenderId = "24" };
            var userId = "12";
            var token = string.Empty;

            string fileName = "qwerty";
            var bytes = Encoding.UTF8.GetBytes("This is a qwerty file wih no extension");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);

            // Act
            var newFileName = await this.fileService.SaveFile(stubFile, fileDto);

            // Assert
            var savedFile = await this.fileService.RetrieveFile(newFileName, userId, token);
            Assert.NotNull(savedFile.Values);
        }

        [Fact]
        public async void CheckIfSavedFilePresent()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverId = "12", SenderId = "24" };
            var userId = "12";
            var token = string.Empty;

            string fileName = "dummy.txt";
            var bytes = Encoding.UTF8.GetBytes("This is a dummy text file");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);
            var newFileName = await this.fileService.SaveFile(stubFile, fileDto);

            // Act
            var savedFile = await this.fileService.RetrieveFile(newFileName, userId, token);

            // Assert
            Assert.NotNull(savedFile.Values);
        }

        [Fact]
        public async void CheckIfUnsavedFilePresent()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverId = "12", SenderId = "24" };
            var userId = "12";
            var token = string.Empty;

            string fileName = "azerty.pdf";
            var bytes = Encoding.UTF8.GetBytes("This is a azerty PDF");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);

            var newFileName = await this.fileService.SaveFile(stubFile, fileDto);

            // Act
            var savedFile = await this.fileService.RetrieveFile("qwerty.txt", userId, token);

            // Assert
            File.Delete(Path.GetTempPath() + newFileName);
            Assert.Null(savedFile.SingleOrDefault().Value);
        }

        [Fact]
        public async void DownloadPresentFile()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverId = "12", SenderId = "24" };
            var userId = "12";
            var token = string.Empty;

            string fileName = "qwerty.pdf";
            var bytes = Encoding.UTF8.GetBytes("This is a qwerty file");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);

            var newFileName = await this.fileService.SaveFile(stubFile, fileDto);

            // Act
            var fileBytes = await this.fileService.RetrieveFile(newFileName, userId, token);

            // Assert
            File.Delete(Path.GetTempPath() + newFileName);
            Assert.NotEmpty(fileBytes.Values);
        }

        [Fact]
        public async void DownloadNonPresentFile()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverId = "12", SenderId = "24" };
            var userId = "12";
            var token = string.Empty;

            string fileName = "pineapple.pdf";
            var bytes = Encoding.UTF8.GetBytes("This is a pineapple PDF");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);

            var newFileName = await this.fileService.SaveFile(stubFile, fileDto);

            // Act
            var fileBytes = await this.fileService.RetrieveFile(newFileName, userId, token);

            // Assert
            File.Delete(Path.GetTempPath() + newFileName);
            Assert.Equal(23, fileBytes.SingleOrDefault().Value.Length);
        }
    }
}