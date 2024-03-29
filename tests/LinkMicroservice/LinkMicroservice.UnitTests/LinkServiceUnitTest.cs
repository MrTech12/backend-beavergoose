﻿using LinkMicroservice.DTOs;
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
    public class LinkServiceUnitTest
    {
        private LinkService linkService;

        public LinkServiceUnitTest()
        {
            this.linkService = new LinkService(new StubLinkRepository());
        }

        [Fact]
        public async Task CreateLink()
        {
            // Arrange
            var fileDto = new FileDTO() { FileName = "qwerty.txt", SenderId = "qw", ReceiverId = "we", AllowedDownloads = 1 };

            // Act
            await this.linkService.CreateSaveLink(fileDto);

            // Assert
            var link = await this.linkService.RetrieveFileName("azerty145");
            Assert.NotNull(link);
        }

        [Fact]
        public async Task CheckIfLinkPresent()
        {
            // Arrange
            var fileDto = new FileDTO() { FileName = "azerty.txt", SenderId = "qw", ReceiverId = "we", AllowedDownloads = 1 };
            await this.linkService.CreateSaveLink(fileDto);

            // Act
            var link = await this.linkService.RetrieveFileName("oranges");

            // Assert
            Assert.NotNull(link);
        }

        [Fact]
        public async Task CheckIfLinkNotPresent()
        {
            // Arrange
            var fileDto = new FileDTO() { FileName = "serty.txt", SenderId = "qw", ReceiverId = "we", AllowedDownloads = 1 };
            await this.linkService.CreateSaveLink(fileDto);

            // Act
            var link = await this.linkService.RetrieveFileName("apples");

            // Assert
            Assert.Equal(String.Empty, link);
        }

        [Fact]
        public async Task RemoveLink()
        {
            // Arrange
            var fileDto = new FileDTO() { FileName = "sandcat.txt", SenderId = "qw", ReceiverId = "we", AllowedDownloads = 1 };
            await this.linkService.CreateSaveLink(fileDto);

            // Act
            await this.linkService.RemoveLink(fileDto);

            // Assert
            var link = await this.linkService.RetrieveFileName("qwerty145");
            Assert.Equal(String.Empty, link);
        }

        [Fact]
        public async Task RetrieveLinksByKnownReceiverId()
        {
            // Arrange

            // Act
            var links = await this.linkService.RetrieveLinks("Flamingo");

            // Assert
            Assert.NotEmpty(links);
        }

        [Fact]
        public async Task RetrieveLinksByUNknownReceiverId()
        {
            // Arrange
            var fileDto = new FileDTO() { FileName = "sandcat.txt", SenderId = "qw", ReceiverId = "we", AllowedDownloads = 1 };
            await this.linkService.CreateSaveLink(fileDto);

            // Act
            var links = await this.linkService.RetrieveLinks("Qwerty");

            // Assert
            Assert.Empty(links);
        }
    }
}
