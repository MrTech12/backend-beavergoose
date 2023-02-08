using Common.Configuration.Helpers;
using DeleteFileApp.DTOs;
using DeleteFileApp.Interfaces;
using DeleteFileApp.Types;

namespace DeleteFileApp.Services
{
    public class DeleteFileService
    {
        private readonly IFileProvider _fileProvider;
        private readonly ILogger _logger;

        public DeleteFileService(IFileProvider fileProvider, ILogger<DeleteFileService> logger)
        {
            _fileProvider = fileProvider;
            _logger = logger;
        }

        public async Task<ResultType> RemoveFile(DeleteFileDTO deleteFileDto)
        {
            var DODataConfig = retrieveDODataConfig();

            var result = await _fileProvider.FindFileAsync(deleteFileDto.FileName, DODataConfig);
            if (!result.SingleOrDefault().Key)
            {
                return ResultType.FileNotFound;
            }
            else if (result.SingleOrDefault().Value != deleteFileDto.UserId)
            {
                _logger.LogInformation("File does not belong to the user of the current request.");
                return ResultType.FileNotForUser;
            }

            await _fileProvider.DeleteFileAsync(deleteFileDto.FileName, DODataConfig);
            return ResultType.FileRemoved;
        }

        internal DigitalOceanAccessConfigDTO retrieveDODataConfig()
        {
            var DoDataConfig = new DigitalOceanAccessConfigDTO()
            {
                ServiceURL = ConfigHelper.GetConfigValue("DigitalOcean", "ServiceURL"),
                BucketName = ConfigHelper.GetConfigValue("DigitalOcean", "BucketName"),
                AccessKey = ConfigHelper.GetConfigValue("DigitalOcean", "AccessKey"),
                SecretAccessKey = ConfigHelper.GetConfigValue("DigitalOcean", "SecretAccessKey")
            };
            return DoDataConfig;
        }
    }
}
