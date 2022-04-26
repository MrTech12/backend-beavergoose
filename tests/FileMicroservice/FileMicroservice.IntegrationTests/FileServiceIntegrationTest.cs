using FileMicroservice.DTOs;
using FileMicroservice.IntegrationTests.Stubs;
using FileMicroservice.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
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
            this.fileService = new FileService(this._configuration, new LocalstackFileProvider(), new StubMessagingProducer());
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

            var bytes = Encoding.UTF8.GetBytes("This is a qwerty file wih no extension");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "qwerty")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

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

            var bytes = Encoding.UTF8.GetBytes("This is a dummy text file");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "dummy.txt")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

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

            var bytes = Encoding.UTF8.GetBytes("This is a azerty PDF");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "azerty.pdf")
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };

            var newFileName = await fileService.SaveFile(stubFile, fileDto);

            // Act
            var savedFile = await fileService.RetrieveFile("qwerty.txt");

            // Assert
            Assert.Null(savedFile);
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

            var newFileName = await fileService.SaveFile(stubFile, fileDto);

            // Act
            await fileService.RemoveFile(newFileName);

            // Assert
            var deletedFile = await fileService.RetrieveFile(newFileName);
            Assert.Null(deletedFile);
        }
    }
}