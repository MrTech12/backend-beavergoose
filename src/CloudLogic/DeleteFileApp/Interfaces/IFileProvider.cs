using DeleteFileApp.DTOs;

namespace DeleteFileApp.Interfaces
{
    public interface IFileProvider
    {
        Task<Dictionary<bool, string>> FindFileAsync(string fileName, DigitalOceanDataConfigDTO DODataConfigDto);

        Task DeleteFileAsync(string fileName, DigitalOceanDataConfigDTO DODataConfigDto);
    }
}
