using DeleteFileApp.DTOs;

namespace DeleteFileApp.Interfaces
{
    public interface IFileProvider
    {
        Task<Dictionary<bool, string>> FindFileAsync(string fileName, DigitalOceanAccessConfigDTO DODataConfigDto);

        Task DeleteFileAsync(string fileName, DigitalOceanAccessConfigDTO DODataConfigDto);
    }
}
