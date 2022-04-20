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

        public async Task SaveFile(IFormFile file)
        {
            DigitalOceanDataConfiguration DODataConfiguration = new DigitalOceanDataConfiguration()
            {
                DOServiceURL = this._configuration["DigitalOcean:ServiceURL"],
                DOBucketName = this._configuration["DigitalOcean:BucketName"],
                DOAccessKey = this._configuration["DigitalOcean:AccessKey"],
                DOSecretAccessKey = this._configuration["DigitalOcean:SecretAccessKey"]
            };

            await this._fileProvider.UploadFileAsync(file, DODataConfiguration);
        }

        public async Task<byte[]?> RetrieveFile(string fileName)
        {
            DigitalOceanDataConfiguration DODataConfiguration = new DigitalOceanDataConfiguration()
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
            DigitalOceanDataConfiguration DODataConfiguration = new DigitalOceanDataConfiguration()
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
