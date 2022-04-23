using LinkMicroservice.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkMicroservice.UnitTests
{
    internal class LinkServiceTest
    {
        private LinkService linkService;
        private readonly TestConfiguration _fixture;
        private readonly IConfiguration _configuration;

        public LinkServiceTest(TestConfiguration fixture)
        {
            this._fixture = fixture;
            this._configuration = _fixture.GetTestDataConfiguration();
        }
    }
}
