using Common.Configuration.Helpers;
using FileMicroservice.DTOs;
using FileMicroservice.Services;
using FileMicroservice.Types;
using FileMicroservice.UnitTests.Stubs;
using Microsoft.AspNetCore.Http;
using Moq;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FileMicroservice.UnitTests
{
    public class FileServiceUnitTest
    {
        private Mock<LocalConfigHelper> fakeConfigHelper;
        private FileService fileService;

        public FileServiceUnitTest()
        {
            this.fileService = new FileService(new StubFileProvider(), new StubMessagingProducer(), new StubDeleteFileHelper(), new StubConfigHelper());
        }

        [Fact]
        public async void SaveFileWithExtension()
        {
            // Arrange
            var uploadFileDto = new UploadFileDTO() { ReceiverId = "12", SenderId = "24", AllowedDownloads = "1" };
            var userId = "12";
            var token = string.Empty;

            string fileName = "dummy.txt";
            var bytes = Encoding.UTF8.GetBytes("This is a dummy text file");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);

            uploadFileDto.File = stubFile;

            // Act
            var newFileName = await this.fileService.SaveFile(uploadFileDto);

            // Assert
            var savedFile = await this.fileService.RetrieveFile(newFileName, userId ,token);
            Assert.NotNull(savedFile.Values);
        }

        [Fact]
        public async void SaveFileWithoutExtension()
        {
            // Arrange
            var uploadFileDto = new UploadFileDTO() { ReceiverId = "12", SenderId = "24", AllowedDownloads = "1" };
            var userId = "12";
            var token = string.Empty;

            string fileName = "qwerty";
            var bytes = Encoding.UTF8.GetBytes("This is a qwerty file wih no extension");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);

            uploadFileDto.File = stubFile;

            // Act
            var newFileName = await this.fileService.SaveFile(uploadFileDto);

            // Assert
            var savedFile = await this.fileService.RetrieveFile(newFileName, userId, token);
            Assert.NotNull(savedFile.Values);
        }

        [Fact]
        public async void CheckIfSavedFilePresent()
        {
            // Arrange
            var uploadFileDto = new UploadFileDTO() { ReceiverId = "12", SenderId = "24", AllowedDownloads = "1" };
            var userId = "12";
            var token = string.Empty;

            string fileName = "dummy.txt";
            var bytes = Encoding.UTF8.GetBytes("This is a dummy text file");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);

            uploadFileDto.File = stubFile;

            var newFileName = await this.fileService.SaveFile(uploadFileDto);

            // Act
            var savedFile = await this.fileService.RetrieveFile(newFileName, userId, token);

            // Assert
            Assert.NotNull(savedFile.Values);
        }

        [Fact]
        public async void CheckIfUnsavedFilePresent()
        {
            // Arrange
            var uploadFileDto = new UploadFileDTO() { ReceiverId = "12", SenderId = "24", AllowedDownloads = "1" };
            var userId = "12";
            var token = string.Empty;

            string fileName = "azerty.pdf";
            var bytes = Encoding.UTF8.GetBytes("This is a azerty PDF");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);

            uploadFileDto.File = stubFile;

            var newFileName = await this.fileService.SaveFile(uploadFileDto);

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
            var uploadFileDto = new UploadFileDTO() { ReceiverId = "12", SenderId = "24", AllowedDownloads = "1" };
            var userId = "12";
            var token = string.Empty;

            string fileName = "qwerty.pdf";
            var bytes = Encoding.UTF8.GetBytes("This is a qwerty file");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);

            uploadFileDto.File = stubFile;

            var newFileName = await this.fileService.SaveFile(uploadFileDto);

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
            var uploadFileDto = new UploadFileDTO() { ReceiverId = "12", SenderId = "24" , AllowedDownloads = "1" };
            var userId = "12";
            var token = string.Empty;

            string fileName = "pineapple.pdf";
            var bytes = Encoding.UTF8.GetBytes("This is a pineapple PDF");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);

            uploadFileDto.File = stubFile;

            var newFileName = await this.fileService.SaveFile(uploadFileDto);

            // Act
            var fileBytes = await this.fileService.RetrieveFile("DictionaryCopy.pdf", userId, token);

            // Assert
            File.Delete(Path.GetTempPath() + newFileName);
            Assert.Equal(ResultType.FileNotFound, fileBytes.SingleOrDefault().Key);
        }

        [Fact]
        public async void DownloadPresentFileFromAnotherUser()
        {
            // Arrange
            var uploadFileDto = new UploadFileDTO() { ReceiverId = "12", SenderId = "24", AllowedDownloads = "1" };
            var userId = "36";
            var token = string.Empty;

            string fileName = "Mango.pdf";
            var bytes = Encoding.UTF8.GetBytes("This is a Mango PDF");
            IFormFile stubFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", fileName);

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