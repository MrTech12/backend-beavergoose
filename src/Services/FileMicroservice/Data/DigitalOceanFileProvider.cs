using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using FileMicroservice.DTOs;
using FileMicroservice.Interfaces;

namespace FileMicroservice.Data
{
	public class DigitalOceanFileProvider : IFileProvider
	{
		private readonly ILogger _logger;

		public DigitalOceanFileProvider(ILogger<DigitalOceanFileProvider> logger)
        {
			this._logger = logger;
		}

		public async Task<byte[]> DownloadFileAsync(string fileName, DigitalOceanDataConfigDTO DODataConfigDto)
		{
			var _awsS3Client = CreateAWSS3Client(DODataConfigDto);

			MemoryStream memoryStream;

			try
			{
				var getRequest = new GetObjectRequest()
                {
					BucketName = DODataConfigDto.DOBucketName,
					Key = fileName // Keys are the full filename, including the file extension.
				};

				this._logger.LogInformation("Downloading a file from DigitalOcean Spaces");
				var response = await _awsS3Client.GetObjectAsync(getRequest);

				using (memoryStream = new MemoryStream())
				{
					await response.ResponseStream.CopyToAsync(memoryStream);
				}

				return memoryStream.ToArray();
			}
			catch (Exception exception)
			{
				this._logger.LogError(exception, "There was a problem when downloading a file. Source of Exception: {Source}. Expection Message: {Message}", exception.Source, exception.Message);
				throw;
			}
		}

        public async Task UploadFileAsync(SaveFileDTO saveFileDto, DigitalOceanDataConfigDTO DODataConfigDto)
		{
			var _awsS3Client = CreateAWSS3Client(DODataConfigDto);

			try
			{
				using (var newMemoryStream = new MemoryStream())
				{
					saveFileDto.File.CopyTo(newMemoryStream);

					var fileTransferUtility = new TransferUtility(_awsS3Client);
					var uploadRequest = new TransferUtilityUploadRequest
					{
						InputStream = newMemoryStream,
						Key = saveFileDto.FileName,
						ContentType = saveFileDto.File.ContentType,
						BucketName = DODataConfigDto.DOBucketName,
						CannedACL = S3CannedACL.Private
					};

					uploadRequest.Metadata.Add("senderid", saveFileDto.SenderId);
					uploadRequest.Metadata.Add("receiverid", saveFileDto.ReceiverId);
					uploadRequest.Metadata.Add("alloweddownloads", saveFileDto.AllowedDownloads);

					this._logger.LogInformation("Uploading a file to DigitalOcean Spaces");
					await fileTransferUtility.UploadAsync(uploadRequest);
				}
			}
			catch (AmazonS3Exception exception)
			{
				this._logger.LogError(exception, "There was an S3 problem when uploading a file. Source of Exception: {Source}. Expection Message: {Message}", exception.Source, exception.Message);
			}
			catch (Exception exception)
			{
				this._logger.LogError(exception, "There was a problem when uploading a file. Source of Exception: {Source}. Expection Message: {Message}", exception.Source, exception.Message);
			}
		}

		public async Task<Dictionary<bool, string>> FindFileAsync(string fileName, DigitalOceanDataConfigDTO DODataConfigDto)
		{
			var _awsS3Client = CreateAWSS3Client(DODataConfigDto);

			try
            {
                var getRequest = new GetObjectRequest()
                {
                    BucketName = DODataConfigDto.DOBucketName,
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
					if (exception.Message.Contains("NoSuchBucket")) {
						return new Dictionary<bool, string>() { { false, string.Empty } };
					}

					else if (exception.Message.Contains("NoSuchKey")) {
						return new Dictionary<bool, string>() { { false, string.Empty } };
					}
				}
				this._logger.LogError(exception, "There was a problem when looking up a file. Source of Exception: {Source}. Expection Message: {Message}", exception.Source, exception.Message);
				throw;
			}
		}

		internal IAmazonS3 CreateAWSS3Client(DigitalOceanDataConfigDTO DODataConfigDto)
        {
			var s3ClientConfig = new AmazonS3Config { ServiceURL = DODataConfigDto.DOServiceURL };
			IAmazonS3 _awsS3Client = new AmazonS3Client(DODataConfigDto.DOAccessKey, DODataConfigDto.DOSecretAccessKey, s3ClientConfig);
			return _awsS3Client;
		}
    }
}
