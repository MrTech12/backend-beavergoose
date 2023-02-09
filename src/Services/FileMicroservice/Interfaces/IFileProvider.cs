using FileMicroservice.DTOs;

namespace FileMicroservice.Interfaces
{
    public interface IFileProvider
    {
        Task<byte[]> DownloadFileAsync(string fileName, AccessConfigDTO AccessConfigDto);

        Task UploadFileAsync(SaveFileDTO saveFileDto, AccessConfigDTO AccessConfigDto);

        Task<Dictionary<bool, string>> FindFileAsync(string fileName, AccessConfigDTO AccessConfigDto);
    }
}
