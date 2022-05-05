using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using FileMicroservice.DTOs;
using FileMicroservice.Interfaces;

namespace FileMicroservice.Data
{
	public class DigitalOceanFileProvider : IFileProvider
	{
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
				var response = await _awsS3Client.GetObjectAsync(getRequest);

				using (memoryStream = new MemoryStream())
				{
					await response.ResponseStream.CopyToAsync(memoryStream);
				}

				return memoryStream.ToArray();
			}
			catch (Exception exception)
			{
				Console.WriteLine("Unknown encountered on server. Message:'{0}' when looking up a file", exception.Message);
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
					await fileTransferUtility.UploadAsync(uploadRequest);
				}
			}
			catch (AmazonS3Exception e)
			{
				Console.WriteLine("Error encountered on server. Message:'{0}' when writing a file", e.Message);
			}
			catch (Exception e)
			{
				Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing a file", e.Message);
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
				Console.WriteLine("Unknown encountered on server. Message:'{0}' when looking up a file", exception.Message);
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
				await _awsS3Client.DeleteObjectAsync(deleteObjectRequest);
			}
			catch (Exception exception)
			{
				Console.WriteLine("Unknown encountered on server. Message:'{0}' when deleting a file", exception.Message);
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
