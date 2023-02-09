using Common.Configuration.Helpers;
using Common.Configuration.Interfaces;
using FileMicroservice.DTOs;
using FileMicroservice.Interfaces;
using FileMicroservice.Types;

namespace FileMicroservice.Services
{
    public class FileService
    {
        private readonly IFileProvider _fileProvider;
        private readonly IMessagingProducer _messagingProducer;
        private readonly IDeleteFileHelper _deleteFileHelper;
        private readonly IConfigHelper _configHelper;

        public FileService(IFileProvider fileProvider, IMessagingProducer messagingProducer, IDeleteFileHelper _deleteFileHelper, IConfigHelper configHelper)
        {
            this._fileProvider = fileProvider;
            this._messagingProducer = messagingProducer;
            this._deleteFileHelper = _deleteFileHelper;
            this._configHelper = configHelper;
        }

        public async Task<string> SaveFile(UploadFileDTO uploadFileDto)
        {
            var DODataConfig = retrieveDODataConfig();

            var saveFileDto = new SaveFileDTO()
            {
                File = uploadFileDto.File,
                SenderId = uploadFileDto.SenderId,
                ReceiverId = uploadFileDto.ReceiverId,
                AllowedDownloads = uploadFileDto.AllowedDownloads
            };

            saveFileDto.FileName = Guid.NewGuid().ToString();
            string fileExtension = Path.GetExtension(uploadFileDto.File.FileName);
            if (fileExtension == string.Empty)
            {
                saveFileDto.FileName += ".txt";
            }
            else
            {
                saveFileDto.FileName += fileExtension;
            }
            await this._fileProvider.UploadFileAsync(saveFileDto, DODataConfig);

            var fileDTO = new FileDTO()
            {
                FileName = saveFileDto.FileName,
                SenderId = saveFileDto.SenderId,
                ReceiverId = saveFileDto.ReceiverId,
                AllowedDownloads = Convert.ToInt32(saveFileDto.AllowedDownloads)
            };
            this._messagingProducer.SendMessage(fileDTO, "create");
            return saveFileDto.FileName;
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

        internal AccessConfigDTO retrieveDODataConfig()
        {
            var accessConfigDto = new AccessConfigDTO()
            {
                ServiceURL = this._configHelper.GetConfigValue("DigitalOcean", "ServiceURL"),
                BucketName = this._configHelper.GetConfigValue("DigitalOcean","BucketName")
            };

            var environmentType = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environmentType == "Development")
            {
                accessConfigDto.AccessKey = this._configHelper.GetConfigValue("DigitalOcean","AccessKey_Dev");
                accessConfigDto.SecretAccessKey = this._configHelper.GetConfigValue("DigitalOcean","SecretAccessKey_Dev");
            }
            else if (environmentType == "Production")
            {
                accessConfigDto.AccessKey = this._configHelper.GetConfigValue("DigitalOcean","AccessKey_Prod");
                accessConfigDto.SecretAccessKey = this._configHelper.GetConfigValue("DigitalOcean", "SecretAccessKey_Prod");
            }

            return accessConfigDto;
        }
    }
}
