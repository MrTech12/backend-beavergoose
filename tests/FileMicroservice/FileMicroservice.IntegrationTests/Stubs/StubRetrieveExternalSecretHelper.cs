using FileMicroservice.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMicroservice.IntegrationTests.Stubs
{
    public class StubRetrieveExternalSecretHelper : IRetrieveExternalSecretHelper
    {
        public string GetSecret(string secretName)
        {
            if (secretName == "DigitalOcean_AccessKey_Dev")
            {
                return "AAAAA";
            }
            else if(secretName == "DigitalOcean_SecretAccessKey_Dev")
            {
                return "AAAAA";
            }
            else if (secretName == "DigitalOcean_ServiceURL")
            {
                return "http://localhost:4566";
            }
            else if (secretName == "DigitalOcean_BucketName")
            {
                return "test-bucket";
            }
            return string.Empty;
        }
    }
}
