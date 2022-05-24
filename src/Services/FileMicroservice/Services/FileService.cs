using FileMicroservice.DTOs;
using FileMicroservice.Helpers;
using FileMicroservice.Interfaces;

namespace FileMicroservice.Services
{
    public class FileService
    {
        private readonly IFileProvider _fileProvider;
        private readonly IConfiguration _configuration;
        private readonly IMessagingProducer _messagingProducer;

        public FileService(IConfiguration configuration, IFileProvider fileProvider, IMessagingProducer _messagingProducer)
        {
            this._configuration = configuration;
            this._fileProvider = fileProvider;
            this._messagingProducer = _messagingProducer;
        }

        public async Task<string> SaveFile(IFormFile file, FileDTO fileDto)
        {
            var DODataConfig = retrieveDODataConfig();

            fileDto.FileName = Guid.NewGuid().ToString();
            string fileExtension = Path.GetExtension(file.FileName);
            if (fileExtension == String.Empty)
            {
                fileDto.FileName += ".txt";
            } else {
                fileDto.FileName += fileExtension;
            }

            await this._fileProvider.UploadFileAsync(file, DODataConfig, fileDto);
            this._messagingProducer.SendMessage(fileDto, "create");

            return fileDto.FileName;
        }

        public async Task<byte[]?> RetrieveFile(string fileName)
        {
            var DODataConfig = retrieveDODataConfig();

            bool presence = await this._fileProvider.FindFileAsync(fileName, DODataConfig);
            if (presence)
            {
                var file = await this._fileProvider.DownloadFileAsync(fileName, DODataConfig);
                
                var fileDto = new FileDTO() { FileName = fileName };
                this._messagingProducer.SendMessage(fileDto, "delete");
                return file;
            }
            return null;
        }

        public async Task RemoveFile(string fileName)
        {
            var DODataConfig = retrieveDODataConfig();
            await this._fileProvider.DeleteFileAsync(fileName, DODataConfig);
        }

        internal DigitalOceanDataConfigDTO retrieveDODataConfig()
        {
            var EmptyDODataConfig = new DigitalOceanDataConfigDTO()
            {
                DOServiceURL = RetrieveConfigHelper.GetConfigValue("DigitalOcean", "ServiceURL"),
                DOBucketName = RetrieveConfigHelper.GetConfigValue("DigitalOcean", "BucketName"),
                DOAccessKey = RetrieveConfigHelper.GetConfigValue("DigitalOcean", "AccessKey"),
                DOSecretAccessKey = RetrieveConfigHelper.GetConfigValue("DigitalOcean", "SecretAccessKey")
            };
            return EmptyDODataConfig;
        }
    }
}
