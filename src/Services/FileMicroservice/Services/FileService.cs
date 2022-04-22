using FileMicroservice.DTOs;
using FileMicroservice.Interfaces;

namespace FileMicroservice.Services
{
    public class FileService
    {
        private readonly IFileProvider _fileProvider;
        private readonly IConfiguration _configuration;

        public FileService(IConfiguration configuration, IFileProvider fileProvider)
        {
            this._configuration = configuration;
            this._fileProvider = fileProvider;
        }

        public async Task<string> SaveFile(IFormFile file)
        {
            DigitalOceanDataConfigurationDTO DODataConfiguration = new DigitalOceanDataConfigurationDTO()
            {
                DOServiceURL = this._configuration["DigitalOcean:ServiceURL"],
                DOBucketName = this._configuration["DigitalOcean:BucketName"],
                DOAccessKey = this._configuration["DigitalOcean:AccessKey"],
                DOSecretAccessKey = this._configuration["DigitalOcean:SecretAccessKey"]
            };

            FileDTO fileDTO = new FileDTO();
            fileDTO.FileName = Guid.NewGuid().ToString();
            string fileExtension = Path.GetExtension(file.FileName);
            if (fileExtension == "")
            {
                fileDTO.FileExtension = ".txt";
            } else {
                fileDTO.FileExtension = fileExtension;
            }

            await this._fileProvider.UploadFileAsync(file, DODataConfiguration, fileDTO);
            return fileDTO.FileName + fileDTO.FileExtension;
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
                return await this._fileProvider.DownloadFileAsync(fileName, DODataConfiguration);
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
    }
}
