using FileMicroservice.DTOs;
using FileMicroservice.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FileMicroservice.UnitTests
{
    internal class StubFileProvider : IFileProvider
    {
        public Task<byte[]> DownloadFileAsync(string fileName, DigitalOceanDataConfiguration DODataConfiguration)
        {
            throw new NotImplementedException();
        }

        public Task<bool> FindFileAsync(string fileName, DigitalOceanDataConfiguration DODataConfiguration)
        {
            if (fileName != "dummy.txt")
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }

        public async Task UploadFileAsync(IFormFile file, DigitalOceanDataConfiguration DODataConfiguration)
        {
            var saveToPath = Path.Combine(Path.GetTempPath(), file.FileName);

            using (var targetStream = File.Create(saveToPath))
            {
                await file.CopyToAsync(targetStream);
            }
        }
    }
}
