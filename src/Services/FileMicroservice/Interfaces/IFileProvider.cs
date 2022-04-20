using FileMicroservice.DTOs;

namespace FileMicroservice.Interfaces
{
    public interface IFileProvider
    {
        Task<byte[]> DownloadFileAsync(string file, DigitalOceanDataConfiguration DODataConfiguration);

        Task UploadFileAsync(IFormFile file, DigitalOceanDataConfiguration DODataConfiguration);
    }
}
