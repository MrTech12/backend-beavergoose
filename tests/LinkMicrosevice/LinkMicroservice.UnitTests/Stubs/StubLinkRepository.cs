using LinkMicroservice.Entities;
using LinkMicroservice.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkMicroservice.UnitTests.Stubs
{
    internal class StubLinkRepository : ILinkRepository
    {
        public Task DeleteLink(Link linkEntity)
        {
            return Task.CompletedTask;
        }

        public Task<Link> RetrieveFileName(string address)
        {
            if (address == "azerty145" || address == "oranges")
            {
                var link1 = new Link() { FileName = "qwerty145" };
                return Task.FromResult(link1);
            }
            var link = new Link();
            return Task.FromResult<Link>(link);
        }

        public Task<Link> RetrieveLink(string fileName)
        {
            if (fileName == "sandcat.txt")
            {
                var link1 = new Link();
                return Task.FromResult(link1);
            }
            var link = new Link();
            return Task.FromResult<Link>(link);
        }

        public Task SaveLink(Link linkEntity)
        {
            return Task.CompletedTask;
        }
    }
}
