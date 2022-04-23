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
    public class FileServiceTest : IClassFixture<TestConfiguration>
    {
        private FileService fileService;
        private readonly TestConfiguration _fixture;
        private readonly IConfiguration _configuration;
        DigitalOceanDataConfigurationDTO StubDataConfigurationDTO;

        public FileServiceTest(TestConfiguration fixture)
        {
            this._fixture = fixture;
            this._configuration = _fixture.GetTestDataConfiguration();

            StubDataConfigurationDTO = new DigitalOceanDataConfigurationDTO()
            {
                DOServiceURL = this._configuration["DigitalOcean:ServiceURL"],
                DOBucketName = this._configuration["DigitalOcean:BucketName"],
                DOAccessKey = this._configuration["DigitalOcean:AccessKey"],
                DOSecretAccessKey = this._configuration["DigitalOcean:SecretAccessKey"]
            };
        }

        [Fact]
        public async void SaveFileWithExtension()
        {
            // Arrange
            FileDTO fileDto = new FileDTO() { ReceiverID = "12", SenderID = "24" };

            string fileName = "dummy.txt";
            var bytes = Encoding.UTF8.GetBytes("This is a dummy file");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);

            fileService = new FileService(this._configuration, new StubFileProvider(), new StubMessagingProducer());

            // Act
            var newFileName = await fileService.SaveFile(stubFile, fileDto);

            // Assert
            var presence = await fileService.CheckPresenceOfFile(newFileName);
            Assert.True(presence);
        }

        [Fact]
        public async void SaveFileWithoutExtension()
        {
            // Arrange
            FileDTO fileDto = new FileDTO() { ReceiverID = "12", SenderID = "24" };

            string fileName = "qwerty";
            var bytes = Encoding.UTF8.GetBytes("This is a qwerty file");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);

            fileService = new FileService(this._configuration, new StubFileProvider(), new StubMessagingProducer());

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
            FileDTO fileDto = new FileDTO() { ReceiverID = "12", SenderID = "24" };

            string fileName = "dummy.txt";
            var bytes = Encoding.UTF8.GetBytes("This is a qwerty file");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);

            fileService = new FileService(this._configuration, new StubFileProvider(), new StubMessagingProducer());
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
            FileDTO fileDto = new FileDTO() { ReceiverID = "12", SenderID = "24" };

            string fileName = "qwerty.pdf";
            var bytes = Encoding.UTF8.GetBytes("This is a qwerty file");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);

            fileService = new FileService(this._configuration, new StubFileProvider(), new StubMessagingProducer());
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
            FileDTO fileDto = new FileDTO() { ReceiverID = "12", SenderID = "24" };

            string fileName = "qwerty.pdf";
            var bytes = Encoding.UTF8.GetBytes("This is a qwerty file");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);

            fileService = new FileService(this._configuration, new StubFileProvider(), new StubMessagingProducer());
            var newFileName = await fileService.SaveFile(stubFile, fileDto);

            // Act
            byte[]? fileBytes = await fileService.RetrieveFile(newFileName);

            // Assert
            Assert.NotEqual(0, fileBytes.Length);
        }

        [Fact]
        public async void DownloadNonPresentFile()
        {
            // Arrange
            FileDTO fileDto = new FileDTO() { ReceiverID = "12", SenderID = "24" };

            string fileName = "qwerty.pdf";
            var bytes = Encoding.UTF8.GetBytes("This is a qwerty file");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);

            fileService = new FileService(this._configuration, new StubFileProvider(), new StubMessagingProducer());
            var newFileName = await fileService.SaveFile(stubFile, fileDto);

            // Act
            byte[]? fileBytes = await fileService.RetrieveFile(newFileName);

            // Assert
            Assert.Equal(21, fileBytes.Length);
        }
    }
}