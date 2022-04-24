using FileMicroservice.DTOs;
using FileMicroservice.IntegrationTests.Stubs;
using FileMicroservice.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FileMicroservice.IntegrationTests
{
    public class FileServiceIntegrationTest : IClassFixture<TestConfiguration>
    {
        private FileService fileService;
        private readonly TestConfiguration _fixture;
        private readonly IConfiguration _configuration;

        public FileServiceIntegrationTest(TestConfiguration fixture)
        {
            this._fixture = fixture;
            this._configuration = _fixture.GetTestDataConfiguration();
        }

        [Fact]
        public async Task SaveFileWithExtension()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverID = "12", SenderID = "24" };

            var bytes = Encoding.UTF8.GetBytes("This is a dummy text file");
            var stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "dummy.txt")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            fileService = new FileService(this._configuration, new LocalstackFileProvider(), new StubMessagingProducer());

            // Act
            var newFileName = await fileService.SaveFile(stubFile, fileDto);

            // Assert
            var presence = await fileService.CheckPresenceOfFile(newFileName);
            Assert.True(presence);
        }

        //TODO: tests to change

        [Fact]
        public async void SaveFileWithoutExtension()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverID = "12", SenderID = "24" };

            var bytes = Encoding.UTF8.GetBytes("This is a qwerty file wih no extension");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "qwerty")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            fileService = new FileService(this._configuration, new LocalstackFileProvider(), new StubMessagingProducer());

            // Act
            var newFileName = await fileService.SaveFile(stubFile, fileDto);

            // Assert
            var presence = await fileService.CheckPresenceOfFile(newFileName);
            Assert.True(presence);
        }

        [Fact]
        public async void CheckIfFilePresent()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverID = "12", SenderID = "24" };

            var bytes = Encoding.UTF8.GetBytes("This is a dummy text file");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "dummy.txt")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            fileService = new FileService(this._configuration, new LocalstackFileProvider(), new StubMessagingProducer());
            var newFileName = await fileService.SaveFile(stubFile, fileDto);

            // Act
            var presence = await fileService.CheckPresenceOfFile(newFileName);

            // Assert
            Assert.True(presence);
        }

        [Fact]
        public async void CheckIfFileNotPresent()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverID = "12", SenderID = "24" };

            var bytes = Encoding.UTF8.GetBytes("This is a azerty PDF");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "azerty.pdf")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };

            fileService = new FileService(this._configuration, new LocalstackFileProvider(), new StubMessagingProducer());
            var newFileName = await fileService.SaveFile(stubFile, fileDto);

            // Act
            var presence = await fileService.CheckPresenceOfFile("qwerty.txt");

            // Assert
            Assert.False(presence);
        }

        [Fact]
        public async void DownloadPresentFile()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverID = "12", SenderID = "24" };

            var bytes = Encoding.UTF8.GetBytes("This is a qwerty file");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "qwerty.pdf")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };

            fileService = new FileService(this._configuration, new LocalstackFileProvider(), new StubMessagingProducer());
            var newFileName = await fileService.SaveFile(stubFile, fileDto);

            // Act
            byte[]? fileBytes = await fileService.RetrieveFile(newFileName);

            // Assert
            Assert.NotEmpty(fileBytes);
        }

        [Fact]
        public async void DownloadNonPresentFile()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverID = "12", SenderID = "24" };

            var bytes = Encoding.UTF8.GetBytes("This is a pineapple PDF");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "pineapple.pdf")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };

            fileService = new FileService(this._configuration, new LocalstackFileProvider(), new StubMessagingProducer());
            var newFileName = await fileService.SaveFile(stubFile, fileDto);

            // Act
            byte[]? fileBytes = await fileService.RetrieveFile(newFileName);

            // Assert
            Assert.Equal(23, fileBytes.Length);
        }

        [Fact]
        public async void RemoveFile()
        {
            // Arrange
            var fileDto = new FileDTO() { ReceiverID = "12", SenderID = "24" };

            var bytes = Encoding.UTF8.GetBytes("This is a qwerty document");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "qwerty.docx")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };

            fileService = new FileService(this._configuration, new LocalstackFileProvider(), new StubMessagingProducer());
            var newFileName = await fileService.SaveFile(stubFile, fileDto);

            // Act
            await fileService.RemoveFile(newFileName);

            // Assert
            bool present = await fileService.CheckPresenceOfFile(newFileName);
            Assert.False(present);
        }
    }
}