using FileMicroservice.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMicroservice.UnitTests.Stubs
{
    public class StubRetrieveConfigHelper : IRetrieveConfigHelper
    {
        public string GetConfigValue(string configurationSection, string configurationKey)
        {
            return "value";
        }
    }
}
