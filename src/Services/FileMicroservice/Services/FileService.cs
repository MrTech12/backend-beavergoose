using FileMicroservice.DTOs;
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
            DigitalOceanDataConfigurationDTO DODataConfiguration = new DigitalOceanDataConfigurationDTO()
            {
                DOServiceURL = this._configuration["DigitalOcean:ServiceURL"],
                DOBucketName = this._configuration["DigitalOcean:BucketName"],
                DOAccessKey = this._configuration["DigitalOcean:AccessKey"],
                DOSecretAccessKey = this._configuration["DigitalOcean:SecretAccessKey"]
            };

            fileDto.FileName = Guid.NewGuid().ToString();
            string fileExtension = Path.GetExtension(file.FileName);
            if (fileExtension == "")
            {
                fileDto.FileExtension = ".txt";
            } else {
                fileDto.FileExtension = fileExtension;
            }

            await this._fileProvider.UploadFileAsync(file, DODataConfiguration, fileDto);
            this._messagingProducer.SendMessage(fileDto, "create");

            return fileDto.FileName + fileDto.FileExtension;
        }

        public async Task<byte[]?> RetrieveFile(string fileName)
        {
            DigitalOceanDataConfigurationDTO DODataConfiguration = new DigitalOceanDataConfigurationDTO()
            {
                DOServiceURL = this._configuration["DigitalOcean:ServiceURL"],
                DOBucketName = this._configuration["DigitalOcean:BucketName"],
                DOAccessKey = this._configuration["DigitalOcean:AccessKey"],
                DOSecretAccessKey = this._configuration["DigitalOcean:SecretAccessKey"]
            };

            bool presence = await this._fileProvider.FindFileAsync(fileName, DODataConfiguration);
            if (presence)
            {
                var file = await this._fileProvider.DownloadFileAsync(fileName, DODataConfiguration);
                FileDTO fileDto = new FileDTO() { FileName = fileName };
                this._messagingProducer.SendMessage(fileDto, "delete");
                return file;
            }
            return null;
        }

        public async Task<bool> CheckPresenceOfFile(string fileName)
        {
            DigitalOceanDataConfigurationDTO DODataConfiguration = new DigitalOceanDataConfigurationDTO()
            {
                DOServiceURL = this._configuration["DigitalOcean:ServiceURL"],
                DOBucketName = this._configuration["DigitalOcean:BucketName"],
                DOAccessKey = this._configuration["DigitalOcean:AccessKey"],
                DOSecretAccessKey = this._configuration["DigitalOcean:SecretAccessKey"]
            };
            return await this._fileProvider.FindFileAsync(fileName, DODataConfiguration);
        }

        public async Task RemoveFile(string fileName)
        {
            DigitalOceanDataConfigurationDTO DODataConfiguration = new DigitalOceanDataConfigurationDTO()
            {
                DOServiceURL = this._configuration["DigitalOcean:ServiceURL"],
                DOBucketName = this._configuration["DigitalOcean:BucketName"],
                DOAccessKey = this._configuration["DigitalOcean:AccessKey"],
                DOSecretAccessKey = this._configuration["DigitalOcean:SecretAccessKey"]
            };
            await this._fileProvider.DeleteFileAsync(fileName, DODataConfiguration);
        }
    }
}
