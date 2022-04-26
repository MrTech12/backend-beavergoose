using FileMicroservice.DTOs;
using FileMicroservice.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FileMicroservice.UnitTests.Stubs
{
    internal class StubFileProvider : IFileProvider
    {
        public Task DeleteFileAsync(string fileName, DigitalOceanDataConfigDTO DODataConfigurationDTO)
        {
            File.Delete(Path.GetTempPath() + fileName);
            return Task.CompletedTask;
        }

        public async Task<byte[]> DownloadFileAsync(string fileName, DigitalOceanDataConfigDTO DODataConfigurationDTO)
        {
            return await File.ReadAllBytesAsync(Path.GetTempPath() + fileName);
        }

        public Task<bool> FindFileAsync(string fileName, DigitalOceanDataConfigDTO DODataConfigurationDTO)
        {
            if (!File.Exists(Path.GetTempPath() + fileName))
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }

        public async Task UploadFileAsync(IFormFile file, DigitalOceanDataConfigDTO DODataConfigurationDTO, FileDTO fileDTO)
        {
            var saveToPath = Path.Combine(Path.GetTempPath(), fileDTO.FileName);

            using (var targetStream = File.Create(saveToPath))
            {
                await file.CopyToAsync(targetStream);
            }
        }
    }
}
