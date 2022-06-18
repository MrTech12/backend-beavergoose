using FileMicroservice.DTOs;
using FileMicroservice.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FileMicroservice.UnitTests.Stubs
{
    internal class StubFileProvider : IFileProvider
    {
        public async Task<byte[]> DownloadFileAsync(string fileName, DigitalOceanDataConfigDTO DODataConfigurationDTO)
        {
            return await File.ReadAllBytesAsync(Path.GetTempPath() + fileName);
        }

        public Task<Dictionary<bool, string>> FindFileAsync(string fileName, DigitalOceanDataConfigDTO DODataConfigurationDTO)
        {
            if (!File.Exists(Path.GetTempPath() + fileName))
            {
                return Task.FromResult(new Dictionary<bool, string>() { { false, string.Empty} });
            }
            return Task.FromResult(new Dictionary<bool, string>() { { true, "24"} });
        }

        public async Task UploadFileAsync(SaveFileDTO saveFileDto, DigitalOceanDataConfigDTO DODataConfigurationDTO)
        {
            var saveToPath = Path.Combine(Path.GetTempPath(), saveFileDto.FileName);

            using (var targetStream = File.Create(saveToPath))
            {
                await saveFileDto.File.CopyToAsync(targetStream);
            }
        }
    }
}
