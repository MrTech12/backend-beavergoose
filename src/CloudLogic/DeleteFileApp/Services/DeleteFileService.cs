using Common.Configuration.Helpers;
using Common.Configuration.Interfaces;
using DeleteFileApp.DTOs;
using DeleteFileApp.Interfaces;
using DeleteFileApp.Types;

namespace DeleteFileApp.Services
{
    public class DeleteFileService
    {
        private readonly IFileProvider _fileProvider;
        private readonly IConfigHelper _configHelper;
        private readonly ILogger _logger;

        public DeleteFileService(IFileProvider fileProvider, IConfigHelper configHelper, ILogger<DeleteFileService> logger)
        {
            _fileProvider = fileProvider;
            _configHelper = configHelper;
            _logger = logger;
        }

        public async Task<ResultType> RemoveFile(DeleteFileDTO deleteFileDto)
        {
            var DODataConfig = retrieveAccessConfig();

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

        internal AccessConfigDTO retrieveAccessConfig()
        {
            var AccessConfig = new AccessConfigDTO()
            {
                ServiceURL = _configHelper.GetConfigValue("DigitalOcean", "ServiceURL"),
                BucketName = _configHelper.GetConfigValue("DigitalOcean", "BucketName"),
                AccessKey = _configHelper.GetConfigValue("DigitalOcean", "AccessKey"),
                SecretAccessKey = _configHelper.GetConfigValue("DigitalOcean", "SecretAccessKey")
            };
            return AccessConfig;
        }
    }
}
