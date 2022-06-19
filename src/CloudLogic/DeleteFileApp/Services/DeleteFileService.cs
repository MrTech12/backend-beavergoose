using DeleteFileApp.DTOs;
using DeleteFileApp.Interfaces;
using DeleteFileApp.Types;

namespace DeleteFileApp.Services
{
    public class DeleteFileService
    {
        private readonly IFileProvider _fileProvider;
        private readonly IRetrieveConfigHelper _retrieveConfigHelper;
        private readonly ILogger _logger;

        public DeleteFileService(IFileProvider fileProvider, IRetrieveConfigHelper retrieveConfigHelper, ILogger<DeleteFileService> logger)
        {
            _fileProvider = fileProvider;
            _retrieveConfigHelper = retrieveConfigHelper;
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

        internal DigitalOceanDataConfigDTO retrieveDODataConfig()
        {
            var EmptyDODataConfig = new DigitalOceanDataConfigDTO()
            {
                DOServiceURL = this._retrieveConfigHelper.GetConfigValue("DigitalOcean", "ServiceURL"),
                DOBucketName = this._retrieveConfigHelper.GetConfigValue("DigitalOcean", "BucketName"),
                DOAccessKey = this._retrieveConfigHelper.GetConfigValue("DigitalOcean", "AccessKey"),
                DOSecretAccessKey = this._retrieveConfigHelper.GetConfigValue("DigitalOcean", "SecretAccessKey")
            };
            return EmptyDODataConfig;
        }
    }
}
