using Amazon.S3;
using Amazon.S3.Model;
using DeleteFileApp.DTOs;
using DeleteFileApp.Interfaces;

namespace DeleteFileApp.Data
{
    public class DigitalOceanFileProvider : IFileProvider
    {
        private readonly ILogger _logger;

        public DigitalOceanFileProvider(ILogger<DigitalOceanFileProvider> logger)
        {
            this._logger = logger;
        }

        public async Task DeleteFileAsync(string fileName, DigitalOceanAccessConfigDTO DOAccessConfigDto)
        {
            var _awsS3Client = CreateAWSS3Client(DOAccessConfigDto);

            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = DOAccessConfigDto.BucketName,
                    Key = fileName // Keys are the full filename, including the file extension.
                };

                this._logger.LogInformation("Deleting a file from DigitalOcean Spaces");
                await _awsS3Client.DeleteObjectAsync(deleteObjectRequest);
            }
            catch (Exception exception)
            {
                this._logger.LogError(exception, "There was a problem when deleting a file. Source of Exception: {Source}. Expection Message: {Message}", exception.Source, exception.Message);
                throw;
            }
        }

        public async Task<Dictionary<bool, string>> FindFileAsync(string fileName, DigitalOceanAccessConfigDTO DOAccessConfigDto)
        {
            var _awsS3Client = CreateAWSS3Client(DOAccessConfigDto);

            try
            {
                var getRequest = new GetObjectRequest()
                {
                    BucketName = DOAccessConfigDto.BucketName,
                    Key = fileName // Keys are the full filename, including the file extension.
                };

                this._logger.LogInformation("Finding a file on DigitalOcean Spaces");
                var result = await _awsS3Client.GetObjectAsync(getRequest);

                this._logger.LogInformation("File found on DigitalOcean Spaces");
                var receiverId = result.Metadata["x-amz-meta-receiverid"].ToString();
                return new Dictionary<bool, string>() { { true, receiverId } };
            }
            catch (Exception exception)
            {
                if (exception.InnerException != null)
                {
                    if (exception.Message.Contains("NoSuchBucket"))
                    {
                        return new Dictionary<bool, string>() { { false, string.Empty } };
                    }

                    else if (exception.Message.Contains("NoSuchKey"))
                    {
                        this._logger.LogInformation("File does not exist in DigitalOcean Spaces");
                        return new Dictionary<bool, string>() { { false, string.Empty } };
                    }
                }
                this._logger.LogError(exception, "There was a problem when looking up a file. Source of Exception: {Source}. Expection Message: {Message}", exception.Source, exception.Message);
                throw;
            }
        }

        internal IAmazonS3 CreateAWSS3Client(DigitalOceanAccessConfigDTO DOAccessConfigDto)
        {
            var s3ClientConfig = new AmazonS3Config { ServiceURL = DOAccessConfigDto.ServiceURL };
            IAmazonS3 _awsS3Client = new AmazonS3Client(DOAccessConfigDto.AccessKey, DOAccessConfigDto.SecretAccessKey, s3ClientConfig);
            return _awsS3Client;
        }
    }
}
