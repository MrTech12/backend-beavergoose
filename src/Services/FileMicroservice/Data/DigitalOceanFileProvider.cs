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
				this._logger.LogError("There was a problem when downloading a file. Source of Exception: {Source}. Expection Message: {Message}", exception.Source, exception.Message);
				throw;
			}
		}

        public async Task UploadFileAsync(IFormFile file, DigitalOceanDataConfigDTO DODataConfigDto, FileDTO fileDto)
		{
			var _awsS3Client = CreateAWSS3Client(DODataConfigDto);

			try
			{
				using (var newMemoryStream = new MemoryStream())
				{
					file.CopyTo(newMemoryStream);

					var fileTransferUtility = new TransferUtility(_awsS3Client);
					var uploadRequest = new TransferUtilityUploadRequest
					{
						InputStream = newMemoryStream,
						Key = fileDto.FileName,
						BucketName = DODataConfigDto.DOBucketName,
						ContentType = file.ContentType,
						CannedACL = S3CannedACL.Private
					};

					uploadRequest.Metadata.Add("senderid", fileDto.SenderID);
					uploadRequest.Metadata.Add("receiverid", fileDto.ReceiverID);
					uploadRequest.Metadata.Add("alloweddownloads", Convert.ToString(fileDto.AllowedDownloads));

					this._logger.LogInformation("Uploading a file to DigitalOcean Spaces");
					await fileTransferUtility.UploadAsync(uploadRequest);
				}
			}
			catch (AmazonS3Exception exception)
			{
				this._logger.LogError("There was an S3 problem when uploading a file. Source of Exception: {Source}. Expection Message: {Message}", exception.Source, exception.Message);
			}
			catch (Exception exception)
			{
				this._logger.LogError("There was a problem when uploading a file. Source of Exception: {Source}. Expection Message: {Message}", exception.Source, exception.Message);
			}
		}

		public async Task<bool> FindFileAsync(string fileName, DigitalOceanDataConfigDTO DODataConfigDto)
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
				await _awsS3Client.GetObjectAsync(getRequest);
				return true;
            }
            catch (Exception exception)
			{
				if (exception.InnerException != null)
				{
					if (exception.Message.Contains("NoSuchBucket")) {
						return false;
					}

					else if (exception.Message.Contains("NoSuchKey")) {
						return false;
					}
				}
				this._logger.LogError("There was a problem when looking up a file. Source of Exception: {Source}. Expection Message: {Message}", exception.Source, exception.Message);
				throw;
			}
		}

        public async Task DeleteFileAsync(string fileName, DigitalOceanDataConfigDTO DODataConfigDto)
        {
			var _awsS3Client = CreateAWSS3Client(DODataConfigDto);

			try
            {
				var deleteObjectRequest = new DeleteObjectRequest
				{
					BucketName = DODataConfigDto.DOBucketName,
					Key = fileName // Keys are the full filename, including the file extension.
				};

				this._logger.LogInformation("Deleting a file from DigitalOcean Spaces");
				await _awsS3Client.DeleteObjectAsync(deleteObjectRequest);
			}
			catch (Exception exception)
			{
				this._logger.LogError("There was a problem when deleting a file. Source of Exception: {Source}. Expection Message: {Message}", exception.Source, exception.Message);
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
