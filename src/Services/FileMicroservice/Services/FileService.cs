using FileMicroservice.DTOs;
using FileMicroservice.Interfaces;
using FileMicroservice.Types;

namespace FileMicroservice.Services
{
    public class FileService
    {
        private readonly IFileProvider _fileProvider;
        private readonly IMessagingProducer _messagingProducer;
        private readonly IRetrieveExternalSecretHelper _retrieveExternalSecretHelper;
        private readonly IDeleteFileHelper _deleteFileHelper;

        public FileService(IFileProvider fileProvider, IMessagingProducer messagingProducer, IRetrieveConfigHelper retrieveConfigHelper, IDeleteFileHelper _deleteFileHelper, IRetrieveExternalSecretHelper retrieveExternalSecretHelper)
        {
            this._fileProvider = fileProvider;
            this._messagingProducer = messagingProducer;
            this._deleteFileHelper = _deleteFileHelper;
            this._retrieveExternalSecretHelper = retrieveExternalSecretHelper;
        }

        public async Task<string> SaveFile(IFormFile file, FileDTO fileDto)
        {
            var DODataConfig = retrieveDODataConfig();

            fileDto.FileName = Guid.NewGuid().ToString();
            string fileExtension = Path.GetExtension(file.FileName);
            if (fileExtension == string.Empty)
            {
                fileDto.FileName += ".txt";
            } else {
                fileDto.FileName += fileExtension;
            }

            await this._fileProvider.UploadFileAsync(file, DODataConfig, fileDto);
            this._messagingProducer.SendMessage(fileDto, "create");

            return fileDto.FileName;
        }

        public async Task<Dictionary<ResultType, byte[]?>> RetrieveFile(string fileName, string userId, string token)
        {
            var DODataConfig = retrieveDODataConfig();

            var presence = await this._fileProvider.FindFileAsync(fileName, DODataConfig);
            if (presence.SingleOrDefault().Key)
            {
                if (presence.SingleOrDefault().Value != userId)
                {
                    return new Dictionary<ResultType, byte[]?>() { { ResultType.FileNotForUser, null } };
                }

                var file = await this._fileProvider.DownloadFileAsync(fileName, DODataConfig);
                
                var fileDto = new FileDTO() { FileName = fileName };
                this._messagingProducer.SendMessage(fileDto, "delete");
                await this._deleteFileHelper.DeleteFile(fileName, token);
                return new Dictionary<ResultType, byte[]?>() { { ResultType.FilePresent, file } };
            }
            return new Dictionary<ResultType, byte[]?>() { { ResultType.FileNotFound, null } };
        }

        internal DigitalOceanDataConfigDTO retrieveDODataConfig()
        {
            var DODataConfig = new DigitalOceanDataConfigDTO()
            {
                DOServiceURL = this._retrieveExternalSecretHelper.GetSecret("DigitalOcean_ServiceURL"),
                DOBucketName = this._retrieveExternalSecretHelper.GetSecret("DigitalOcean_BucketName"),
            };

            var environmentType = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environmentType == "Development")
            {
                DODataConfig.DOAccessKey = this._retrieveExternalSecretHelper.GetSecret("DigitalOcean_AccessKey_Dev");
                DODataConfig.DOSecretAccessKey = this._retrieveExternalSecretHelper.GetSecret("DigitalOcean_SecretAccessKey_Dev");
            }
            else if (environmentType == "Production")
            {
                DODataConfig.DOAccessKey = this._retrieveExternalSecretHelper.GetSecret("DigitalOcean_AccessKey_Prod");
                DODataConfig.DOSecretAccessKey = this._retrieveExternalSecretHelper.GetSecret("DigitalOcean_SecretAccessKey_Prod");
            }

            return DODataConfig;
        }
    }
}
