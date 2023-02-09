using FileMicroservice.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMicroservice.IntegrationTests.Helpers
{
    public class StubDeleteFileHelper : IDeleteFileHelper
    {
        public Task DeleteFile(string fileName, string token)
        {
            return Task.CompletedTask;
        }
    }
}
