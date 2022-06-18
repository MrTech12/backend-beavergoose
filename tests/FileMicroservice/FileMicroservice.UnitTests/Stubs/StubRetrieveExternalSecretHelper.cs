using FileMicroservice.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMicroservice.UnitTests.Stubs
{
    public class StubRetrieveExternalSecretHelper : IRetrieveExternalSecretHelper
    {
        public string GetSecret(string secretName)
        {
            return "";
        }
    }
}
