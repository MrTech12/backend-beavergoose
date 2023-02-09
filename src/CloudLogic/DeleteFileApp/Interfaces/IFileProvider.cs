using DeleteFileApp.DTOs;

namespace DeleteFileApp.Interfaces
{
    public interface IFileProvider
    {
        Task<Dictionary<bool, string>> FindFileAsync(string fileName, AccessConfigDTO accessConfigDto);

        Task DeleteFileAsync(string fileName, AccessConfigDTO accessConfigDto);
    }
}
