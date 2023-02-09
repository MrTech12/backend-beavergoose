using Common.Configuration.Interfaces;

namespace AccountMicroservice.UnitTests.Stubs
{
    public class StubConfigHelper : IConfigHelper
    {
        public string GetConfigValue(string section, string key)
        {
            return "test value";
        }
    }
}
