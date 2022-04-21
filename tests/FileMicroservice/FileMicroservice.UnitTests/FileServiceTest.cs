using FileMicroservice.DTOs;
using FileMicroservice.Services;
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
        DigitalOceanDataConfiguration StubDataConfiguration;

        public FileServiceTest(TestConfiguration fixture)
        {
            this._fixture = fixture;
            this._configuration = _fixture.GetTestDataConfiguration();

            StubDataConfiguration = new DigitalOceanDataConfiguration()
            {
                DOServiceURL = this._configuration["DigitalOcean:ServiceURL"],
                DOBucketName = this._configuration["DigitalOcean:BucketName"],
                DOAccessKey = this._configuration["DigitalOcean:AccessKey"],
                DOSecretAccessKey = this._configuration["DigitalOcean:SecretAccessKey"]
            };
        }

        [Fact]
        public async void SaveFile()
        {
            // Arrange
            var bytes = Encoding.UTF8.GetBytes("This is a dummy file");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "dummy.txt");
            
            fileService = new FileService(this._configuration, new StubFileProvider());

            // Act
            await fileService.SaveFile(stubFile);

            // Assert
            var presence = await fileService.CheckPresenceOfFile("dummy.txt");
            Assert.True(presence);
        }

        // Test where no file extension is present.

        // Test to check if the available file is present

        // Test to check if the unavailable file is not present

        // Test to download a present file.

        // Test to download a non-present file.
    }
}