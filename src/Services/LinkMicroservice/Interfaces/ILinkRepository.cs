using LinkMicroservice.DTOs;
using LinkMicroservice.Entities;

namespace LinkMicroservice.Interfaces
{
    public interface ILinkRepository
    {
        Task<Link> RetrieveFileName(string address);

        Task<Link> RetrieveLink(string fileName);

        Task<List<Link>> RetrieveLinks(string receiverID);

        Task SaveLink(Link linkEntity);

        Task DeleteLink(Link linkEntity);
    }
}
