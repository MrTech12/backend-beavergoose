using LinkMicroservice.Entities;
using LinkMicroservice.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkMicroservice.UnitTests.Stubs
{
    internal class StubLinkRepository : ILinkRepository
    {
        public Task DeleteLink(Link linkEntity)
        {
            throw new NotImplementedException();
        }

        public Task<Link> RetrieveFileName(string address)
        {
            throw new NotImplementedException();
        }

        public Task<Link> RetrieveLink(string fileName)
        {
            throw new NotImplementedException();
        }

        public Task SaveLink(Link linkEntity)
        {
            throw new NotImplementedException();
        }
    }
}
