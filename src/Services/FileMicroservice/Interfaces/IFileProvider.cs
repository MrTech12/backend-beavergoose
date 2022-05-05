using FileMicroservice.DTOs;

namespace FileMicroservice.Interfaces
{
    public interface IFileProvider
    {
        Task<byte[]> DownloadFileAsync(string fileName, DigitalOceanDataConfigDTO DODataConfigDto);

        Task UploadFileAsync(IFormFile file, DigitalOceanDataConfigDTO DODataConfigDto, FileDTO fileDTO);

        Task<bool> FindFileAsync(string fileName, DigitalOceanDataConfigDTO DODataConfigDto);

        Task DeleteFileAsync(string fileName, DigitalOceanDataConfigDTO DODataConfigDto);
    }
}
