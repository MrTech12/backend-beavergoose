using LinkMicroservice.DTOs;
using LinkMicroservice.Services;
using LinkMicroservice.UnitTests.Stubs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LinkMicroservice.UnitTests
{
    public class LinkServiceTest
    {
        private LinkService linkService;

        [Fact]
        public async Task CreateFile()
        {
            // Arrange
            linkService = new LinkService(new StubLinkRepository());
            var fileDto = new FileDTO() { FileName = "qwerty.txt", SenderID = "qw", ReceiverID = "we", AllowedDownloads = 1 };

            // Act
            await linkService.CreateSaveLink(fileDto);

            // Assert
            var link = await linkService.RetrieveFileName("azerty145");
            Assert.NotNull(link);
        }

        [Fact]
        public async Task CheckIfFilePresent()
        {
            // Arrange
            linkService = new LinkService(new StubLinkRepository());
            var fileDto = new FileDTO() { FileName = "azerty.txt", SenderID = "qw", ReceiverID = "we", AllowedDownloads = 1 };
            await linkService.CreateSaveLink(fileDto);

            // Act
            var link = await linkService.RetrieveFileName("oranges");

            // Assert
            Assert.NotNull(link);
        }

        [Fact]
        public async Task CheckIfFileNotPresent()
        {
            // Arrange
            linkService = new LinkService(new StubLinkRepository());
            var fileDto = new FileDTO() { FileName = "serty.txt", SenderID = "qw", ReceiverID = "we", AllowedDownloads = 1 };
            await linkService.CreateSaveLink(fileDto);

            // Act
            var link = await linkService.RetrieveFileName("apples");

            // Assert
            Assert.Equal(String.Empty, link);
        }

        [Fact]
        public async Task RemoveLink()
        {
            // Arrange
            linkService = new LinkService(new StubLinkRepository());
            var fileDto = new FileDTO() { FileName = "sandcat.txt", SenderID = "qw", ReceiverID = "we", AllowedDownloads = 1 };
            await linkService.CreateSaveLink(fileDto);

            // Act
            await linkService.RemoveLink(fileDto);

            // Assert
            var link = await linkService.RetrieveFileName("qwerty145");
            Assert.Equal(String.Empty, link);
        }
    }
}
