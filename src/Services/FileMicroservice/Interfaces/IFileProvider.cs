using FileMicroservice.DTOs;

namespace FileMicroservice.Interfaces
{
    public interface IFileProvider
    {
        Task<byte[]> DownloadFileAsync(string fileName, DigitalOceanDataConfiguration DODataConfiguration);

        Task UploadFileAsync(IFormFile file, DigitalOceanDataConfiguration DODataConfiguration);

        Task<bool> FindFileAsync(string fileName, DigitalOceanDataConfiguration DODataConfiguration);
    }
}
