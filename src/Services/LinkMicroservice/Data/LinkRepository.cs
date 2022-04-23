using LinkMicroservice.Entities;
using LinkMicroservice.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LinkMicroservice.Data
{
    public class LinkRepository : ILinkRepository
    {
        public readonly LinkContext _context;

        public LinkRepository(LinkContext context)
        {
            this._context = context;
        }

        public async Task<Link> RetrieveFileName(string address)
        {
            return await this._context.Links.FirstOrDefaultAsync(link => link.Address == address);
        }

        public async Task<Link> RetrieveLink(string fileName)
        {
            return await this._context.Links.FirstOrDefaultAsync(link => link.FileName == fileName);
        }

        public async Task SaveLink(Link linkEntity)
        {
            await this._context.Links.AddAsync(linkEntity);
            await this._context.SaveChangesAsync();
        }

        public async Task DeleteLink(Link linkEntity)
        {
            this._context.Links.Remove(linkEntity);
            await this._context.SaveChangesAsync();
        }
    }
}
