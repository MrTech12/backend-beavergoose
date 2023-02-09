using Common.Configuration.Interfaces;
using System;

namespace FileMicroservice.UnitTests.Stubs
{
    public class StubConfigHelper : IConfigHelper
    {
        public string GetConfigValue(string section, string key)
        {
            return "test value";
        }
    }
}
