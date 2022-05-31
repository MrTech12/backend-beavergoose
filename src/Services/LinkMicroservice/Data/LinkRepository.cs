using LinkMicroservice.Entities;
using LinkMicroservice.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LinkMicroservice.Data
{
    public class LinkRepository : ILinkRepository
    {
        public readonly LinkContext _context;
        private readonly ILogger _logger;

        public LinkRepository(LinkContext context, ILogger<LinkRepository> logger)
        {
            this._context = context;
            this._logger = logger;
        }

        public async Task<Link> RetrieveFileName(string address)
        {
            try
            {
                this._logger.LogInformation("Retrieving filename based on a given address");
                return await this._context.Links.FirstOrDefaultAsync(link => link.Address == address);
            }
            catch (Exception exception)
            {
                this._logger.LogError("There was a problem getting the filename. Source of Exception: {Source}. Expection Message: {Message}", exception.Source, exception.Message);
                throw;
            }

        }

        public async Task<Link> RetrieveLink(string fileName)
        {
            try
            {
                this._logger.LogInformation("Retrieving link based on a given filename");
                return await this._context.Links.FirstOrDefaultAsync(link => link.FileName == fileName);
            }
            catch (Exception exception)
            {
                this._logger.LogError("There was a problem getting the link. Source of Exception: {Source}. Expection Message: {Message}", exception.Source, exception.Message);
                throw;
            }
            
        }

        public async Task SaveLink(Link linkEntity)
        {
            try
            {
                this._logger.LogInformation("Saving a new link");
                await this._context.Links.AddAsync(linkEntity);
                await this._context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                this._logger.LogError("There was a problem saving a link. Source of Exception: {Source}. Expection Message: {Message}", exception.Source, exception.Message);
                throw;
            }

        }

        public async Task DeleteLink(Link linkEntity)
        {
            try
            {
                this._logger.LogInformation("Removing an existing link");
                this._context.Links.Remove(linkEntity);
                await this._context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                this._logger.LogError("There was a problem deleting a link. Source of Exception: {Source}. Expection Message: {Message}", exception.Source, exception.Message);
                throw;
            }

        }

        public async Task<List<Link>> RetrieveLinks(string receiverID)
        {
            try
            {
                this._logger.LogInformation("Retrieving all links for a given receiver");
                return await this._context.Links.Where(link => link.ReceiverID == receiverID).ToListAsync();
            }
            catch (Exception exception)
            {
                this._logger.LogError("There was a problem getting all link for a given receiver. Source of Exception: {Source}. Expection Message: {Message}", exception.Source, exception.Message);
                throw;
            }
        }
    }
}
