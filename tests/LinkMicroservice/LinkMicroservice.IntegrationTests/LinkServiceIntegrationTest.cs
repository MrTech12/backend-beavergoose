using LinkMicroservice.Data;
using LinkMicroservice.DTOs;
using LinkMicroservice.Entities;
using LinkMicroservice.Services;
using LinkMicroservice.UnitTests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LinkMicroservice.IntegrationTests
{
    public class LinkServiceIntegrationTest: IDisposable, IClassFixture<TestConfiguration>
    {
        private readonly LinkContext _linkContext;
        private readonly LinkRepository _linkRepository;
        private readonly LinkService _linkService;
        private readonly TestConfiguration _fixture;
        private readonly IConfiguration _configuration;

        public LinkServiceIntegrationTest(TestConfiguration fixture)
        {
            this._fixture = fixture;
            this._configuration = _fixture.GetTestDataConfiguration();
            string connectionString = this._configuration.GetConnectionString("LinkContext");

            var serviceProvider = new ServiceCollection().AddEntityFrameworkNpgsql().BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<LinkContext>();
            builder.UseNpgsql(connectionString).UseInternalServiceProvider(serviceProvider);
            this._linkContext = new LinkContext(builder.Options);
            
            this._linkContext.Database.Migrate();

            var loggerMock = new Mock<ILogger<LinkRepository>>();
            ILogger<LinkRepository> linkRepositorylogger = loggerMock.Object;

            this._linkRepository = new LinkRepository(_linkContext, linkRepositorylogger);
            this._linkService = new LinkService(this._linkRepository);
        }

        [Fact]
        public async Task CreateSaveALink()
        {
            // Arrange
            FileDTO fileDto = new FileDTO() { FileName = "qwerty.txt", SenderId = "Jan", ReceiverId = "Bob", AllowedDownloads = 1 };

            // Act
            await this._linkService.CreateSaveLink(fileDto);
            var storedLink = await this._linkContext.Links.Where(l => l.FileName == fileDto.FileName).SingleOrDefaultAsync();

            // Assert
            Assert.NotNull(storedLink);
            Assert.Equal(fileDto.FileName, storedLink.FileName);
        }

        [Fact]
        public async Task RetrieveOneFileName()
        {
            // Arrange
            this._linkContext.Links.Add(new Link() { FileName = "qwerty.txt", Address = "acjfir3-94rqfj94fj", SenderID = "Bert", ReceiverID = "Duck", AllowedDownloads = 1});
            this._linkContext.Links.Add(new Link() { FileName = "azerty.pdf", Address = "rm488-eiwur23umu2n", SenderID = "Bert", ReceiverID = "Duck", AllowedDownloads = 1 });
            await this._linkContext.SaveChangesAsync();

            // Act
            var fileName = await this._linkService.RetrieveFileName("rm488-eiwur23umu2n");

            // Assert
            Assert.NotNull(fileName);
            Assert.Equal("azerty.pdf", fileName);
        }

        [Fact]
        public async Task RetrieveZeroFileNames()
        {
            // Arrange
            this._linkContext.Links.Add(new Link() { FileName = "qwerty.txt", Address = "acjfir3-94rqfj94fj", SenderID = "Bert", ReceiverID = "Duck", AllowedDownloads = 1 });
            this._linkContext.Links.Add(new Link() { FileName = "azerty.pdf", Address = "rm488-eiwur23umu2n", SenderID = "Bert", ReceiverID = "Duck", AllowedDownloads = 1 });
            await this._linkContext.SaveChangesAsync();

            // Act
            var fileName = await this._linkService.RetrieveFileName("fffnjew-ekrweir29312");

            // Assert
            Assert.Null(fileName);
        }

        [Fact]
        public async Task RetrieveOneLink()
        {
            // Arrange
            this._linkContext.Links.Add(new Link() { FileName = "assignment Jan.docx", Address = "rer3u4-ermci333", SenderID = "Jan", ReceiverID = "Mr. Henk", AllowedDownloads = 1 });
            this._linkContext.Links.Add(new Link() { FileName = "assignment Boris.docx", Address = "eqwe2-2ewru383", SenderID = "Borsi", ReceiverID = "Mr. Henk", AllowedDownloads = 1 });
            this._linkContext.Links.Add(new Link() { FileName = "assignment Brigette.docx", Address = "erjiew892-w-wrweu", SenderID = "Brigette", ReceiverID = "Mr. Henk", AllowedDownloads = 1 });
            this._linkContext.Links.Add(new Link() { FileName = "grades Jan.pdf", Address = "rwermo3994-4329r99", SenderID = "Mr. Henk", ReceiverID = "Jan", AllowedDownloads = 1 });
            await this._linkContext.SaveChangesAsync();

            // Act
            var link = await this._linkService.RetrieveLinks("Jan");

            // Assert
            Assert.NotNull(link);
            Assert.True(link.Count == 1);
        }

        [Fact]
        public async Task RetrieveZeroLinks()
        {
            // Arrange
            this._linkContext.Links.Add(new Link() { FileName = "assignment Jan.docx", Address = "rer3u4-ermci333", SenderID = "Jan", ReceiverID = "Mr. Henk", AllowedDownloads = 1 });
            this._linkContext.Links.Add(new Link() { FileName = "assignment Boris.docx", Address = "eqwe2-2ewru383", SenderID = "Borsi", ReceiverID = "Mr. Henk", AllowedDownloads = 1 });
            this._linkContext.Links.Add(new Link() { FileName = "assignment Brigette.docx", Address = "erjiew892-w-wrweu", SenderID = "Brigette", ReceiverID = "Mr. Henk", AllowedDownloads = 1 });
            this._linkContext.Links.Add(new Link() { FileName = "grades Jan.pdf", Address = "rwermo3994-4329r99", SenderID = "Mr. Henk", ReceiverID = "Jan", AllowedDownloads = 1 });
            await this._linkContext.SaveChangesAsync();

            // Act
            var links = await this._linkService.RetrieveLinks("Linus");

            // Assert
            Assert.True(links.Count == 0);
        }

        [Fact]
        public async Task RetrieveManyLinks()
        {
            // Arrange
            this._linkContext.Links.Add(new Link() { FileName = "assignment Jan.docx", Address = "rer3u4-ermci333", SenderID = "Jan", ReceiverID = "Mr. Henk", AllowedDownloads = 1 });
            this._linkContext.Links.Add(new Link() { FileName = "assignment Boris.docx", Address = "eqwe2-2ewru383", SenderID = "Borsi", ReceiverID = "Mr. Henk", AllowedDownloads = 1 });
            this._linkContext.Links.Add(new Link() { FileName = "assignment Brigette.docx", Address = "erjiew892-w-wrweu", SenderID = "Brigette", ReceiverID = "Mr. Henk", AllowedDownloads = 1 });
            this._linkContext.Links.Add(new Link() { FileName = "grades Jan.pdf", Address = "rwermo3994-4329r99", SenderID = "Mr. Henk", ReceiverID = "Jan", AllowedDownloads = 1 });
            await this._linkContext.SaveChangesAsync();

            // Act
            var links = await this._linkService.RetrieveLinks("Mr. Henk");

            // Assert
            Assert.True(links.Count == 3);
        }

        [Fact]
        public async Task RemoveOneLink()
        {
            // Arrange
            this._linkContext.Links.Add(new Link() { FileName = "assignment Jan.docx", Address = "rer3u4-ermci333", SenderID = "Jan", ReceiverID = "Mr. Henk", AllowedDownloads = 1 });
            this._linkContext.Links.Add(new Link() { FileName = "assignment Boris.docx", Address = "eqwe2-2ewru383", SenderID = "Borsi", ReceiverID = "Mr. Henk", AllowedDownloads = 1 });
            this._linkContext.Links.Add(new Link() { FileName = "assignment Brigette.docx", Address = "erjiew892-w-wrweu", SenderID = "Brigette", ReceiverID = "Mr. Henk", AllowedDownloads = 1 });
            this._linkContext.Links.Add(new Link() { FileName = "grades Jan.pdf", Address = "rwermo3994-4329r99", SenderID = "Mr. Henk", ReceiverID = "Jan", AllowedDownloads = 1 });
            await this._linkContext.SaveChangesAsync();

            FileDTO deletedFile = new FileDTO() { FileName = "assignment Jan.docx", SenderId = "Jan", ReceiverId = "Mr. Henk", AllowedDownloads = 1 };

            // Act
            await this._linkService.RemoveLink(deletedFile);
            var fileName = await this._linkService.RetrieveFileName("rer3u4-ermci333");

            // Assert
            Assert.Null(fileName);
        }

        [Fact]
        public async Task RemoveZeroLinks()
        {
            // Arrange
            this._linkContext.Links.Add(new Link() { FileName = "assignment Jan.docx", Address = "rer3u4-ermci333", SenderID = "Jan", ReceiverID = "Mr. Henk", AllowedDownloads = 1 });
            this._linkContext.Links.Add(new Link() { FileName = "assignment Boris.docx", Address = "eqwe2-2ewru383", SenderID = "Borsi", ReceiverID = "Mr. Henk", AllowedDownloads = 1 });
            this._linkContext.Links.Add(new Link() { FileName = "assignment Brigette.docx", Address = "erjiew892-w-wrweu", SenderID = "Brigette", ReceiverID = "Mr. Henk", AllowedDownloads = 1 });
            this._linkContext.Links.Add(new Link() { FileName = "grades Jan.pdf", Address = "rwermo3994-4329r99", SenderID = "Mr. Henk", ReceiverID = "Jan", AllowedDownloads = 1 });
            await this._linkContext.SaveChangesAsync();

            FileDTO deletedFile = new FileDTO() { FileName = "assignment Adrian.docx", SenderId = "Adrian", ReceiverId = "Mr. Henk", AllowedDownloads = 1 };

            // Act
            await this._linkService.RemoveLink(deletedFile);
            var links = await this._linkService.RetrieveLinks("Mr. Henk");

            // Assert
            Assert.True(links.Count == 3);
        }

        public void Dispose()
        {
            this._linkContext.Database.EnsureDeleted();
        }
    }
}