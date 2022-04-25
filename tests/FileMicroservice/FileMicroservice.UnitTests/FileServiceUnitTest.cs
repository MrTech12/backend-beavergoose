using FileMicroservice.DTOs;
using FileMicroservice.Services;
using FileMicroservice.UnitTests.Stubs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text;
using Xunit;

namespace FileMicroservice.UnitTests
{
    public class FileServiceUnitTest : IClassFixture<TestConfiguration>
    {
        private FileService fileService;
        private readonly TestConfiguration _fixture;
        private readonly IConfiguration _configuration;

        public FileServiceUnitTest(TestConfiguration fixture)
        {
            this._fixture = fixture;
            this._configuration = _fixture.GetTestDataConfiguration();
        }

        [Fact]
        public async void SaveFileWithExtension()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverID = "12", SenderID = "24" };

            string fileName = "dummy.txt";
            var bytes = Encoding.UTF8.GetBytes("This is a dummy text file");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);

            fileService = new FileService(this._configuration, new StubFileProvider(), new StubMessagingProducer());

            // Act
            var newFileName = await fileService.SaveFile(stubFile, fileDto);

            // Assert
            var savedFile = await fileService.RetrieveFile(newFileName);
            Assert.NotNull(savedFile);
        }

        [Fact]
        public async void SaveFileWithoutExtension()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverID = "12", SenderID = "24" };

            string fileName = "qwerty";
            var bytes = Encoding.UTF8.GetBytes("This is a qwerty file wih no extension");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);

            fileService = new FileService(this._configuration, new StubFileProvider(), new StubMessagingProducer());

            // Act
            var newFileName = await fileService.SaveFile(stubFile, fileDto);

            // Assert
            var savedFile = await fileService.RetrieveFile(newFileName);
            Assert.NotNull(savedFile);
        }

        [Fact]
        public async void CheckIfSavedFilePresent()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverID = "12", SenderID = "24" };

            string fileName = "dummy.txt";
            var bytes = Encoding.UTF8.GetBytes("This is a dummy text file");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);

            fileService = new FileService(this._configuration, new StubFileProvider(), new StubMessagingProducer());
            var newFileName = await fileService.SaveFile(stubFile, fileDto);

            // Act
            var savedFile = await fileService.RetrieveFile(newFileName);

            // Assert
            Assert.NotNull(savedFile);
        }

        [Fact]
        public async void CheckIfUnsavedFilePresent()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverID = "12", SenderID = "24" };

            string fileName = "azerty.pdf";
            var bytes = Encoding.UTF8.GetBytes("This is a azerty PDF");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);

            fileService = new FileService(this._configuration, new StubFileProvider(), new StubMessagingProducer());
            var newFileName = await fileService.SaveFile(stubFile, fileDto);

            // Act
            var savedFile = await fileService.RetrieveFile("qwerty.txt");

            // Assert
            File.Delete(Path.GetTempPath() + newFileName);
            Assert.Null(savedFile);
        }

        [Fact]
        public async void DownloadPresentFile()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverID = "12", SenderID = "24" };

            string fileName = "qwerty.pdf";
            var bytes = Encoding.UTF8.GetBytes("This is a qwerty file");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);

            fileService = new FileService(this._configuration, new StubFileProvider(), new StubMessagingProducer());
            var newFileName = await fileService.SaveFile(stubFile, fileDto);

            // Act
            byte[]? fileBytes = await fileService.RetrieveFile(newFileName);

            // Assert
            File.Delete(Path.GetTempPath() + newFileName);
            Assert.NotEmpty(fileBytes);
        }

        [Fact]
        public async void DownloadNonPresentFile()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverID = "12", SenderID = "24" };

            string fileName = "pineapple.pdf";
            var bytes = Encoding.UTF8.GetBytes("This is a pineapple PDF");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);

            fileService = new FileService(this._configuration, new StubFileProvider(), new StubMessagingProducer());
            var newFileName = await fileService.SaveFile(stubFile, fileDto);

            // Act
            byte[]? fileBytes = await fileService.RetrieveFile(newFileName);

            // Assert
            File.Delete(Path.GetTempPath() + newFileName);
            Assert.Equal(23, fileBytes.Length);
        }

        [Fact]
        public async void RemoveFile()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverID = "12", SenderID = "24" };

            string fileName = "qwerty.docx";
            var bytes = Encoding.UTF8.GetBytes("This is a qwerty document");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);

            fileService = new FileService(this._configuration, new StubFileProvider(), new StubMessagingProducer());
            var newFileName = await fileService.SaveFile(stubFile, fileDto);

            // Act
            await fileService.RemoveFile(newFileName);

            // Assert
            File.Delete(Path.GetTempPath() + newFileName);
            var deletedFile = await fileService.RetrieveFile(newFileName);
            Assert.Null(deletedFile);
        }
    }
}