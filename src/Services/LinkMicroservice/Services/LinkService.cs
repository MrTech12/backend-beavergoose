using LinkMicroservice.DTOs;
using LinkMicroservice.Entities;
using LinkMicroservice.Interfaces;

namespace LinkMicroservice.Services
{
    public class LinkService
    {
        private readonly ILinkRepository _linkRepository;
        public LinkService(ILinkRepository linkRepository)
        {
            this._linkRepository = linkRepository;
        }

        public async Task CreateSaveLink(FileDTO fileDto)
        {
            Link link = new Link()
            {
                Address = Guid.NewGuid().ToString(),
                FileName = fileDto.FileName + fileDto.FileExtension,
                SenderID = fileDto.SenderID,
                ReceiverID = fileDto.ReceiverID
            };
            await this._linkRepository.SaveLink(link);
        }

        public async Task<string?> RetrieveFileName(string address)
        {
            var link = await this._linkRepository.RetrieveFileName(address);
            if (link == null)
            {
                return null;
            }
            return link.FileName;
        }

        public async Task RemoveLink(FileDTO fileDto)
        {
            string fileName = fileDto.FileName + fileDto.FileExtension;
            var link = await this._linkRepository.RetrieveLink(fileName);
            await this._linkRepository.DeleteLink(link);
        }
    }
}
