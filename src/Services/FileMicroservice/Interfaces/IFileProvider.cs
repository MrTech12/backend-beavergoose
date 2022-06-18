using FileMicroservice.DTOs;

namespace FileMicroservice.Interfaces
{
    public interface IFileProvider
    {
        Task<byte[]> DownloadFileAsync(string fileName, DigitalOceanDataConfigDTO DODataConfigDto);

        Task UploadFileAsync(SaveFileDTO saveFileDto, DigitalOceanDataConfigDTO DODataConfigDto);

        Task<Dictionary<bool, string>> FindFileAsync(string fileName, DigitalOceanDataConfigDTO DODataConfigDto);
    }
}
