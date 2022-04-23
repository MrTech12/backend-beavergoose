using FileMicroservice.DTOs;

namespace FileMicroservice.Interfaces
{
    public interface IFileProvider
    {
        Task<byte[]> DownloadFileAsync(string fileName, DigitalOceanDataConfigurationDTO DODataConfigurationDTO);

        Task UploadFileAsync(IFormFile file, DigitalOceanDataConfigurationDTO DODataConfigurationDTO, FileDTO fileDTO);

        Task<bool> FindFileAsync(string fileName, DigitalOceanDataConfigurationDTO DODataConfigurationDTO);

        Task DeleteFileAsync(string fileName, DigitalOceanDataConfigurationDTO DODataConfigurationDTO);
    }
}
