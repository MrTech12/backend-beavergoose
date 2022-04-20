using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using FileMicroservice.DTOs;
using FileMicroservice.Interfaces;

namespace FileMicroservice.Data
{
	public class DigitalOceanFileProvider : IFileProvider
	{
		public async Task<byte[]> DownloadFileAsync(string fileName, DigitalOceanDataConfiguration DODataConfiguration)
		{
			var s3ClientConfig = new AmazonS3Config { ServiceURL = DODataConfiguration.DOServiceURL };
			IAmazonS3 _awsS3Client = new AmazonS3Client(DODataConfiguration.DOAccessKey, DODataConfiguration.DOSecretAccessKey, s3ClientConfig);

			MemoryStream memoryStream = null;

			try
			{
				GetObjectRequest getRequest = new GetObjectRequest
				{
					BucketName = DODataConfiguration.DOBucketName,
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

        public async Task UploadFileAsync(IFormFile file, DigitalOceanDataConfiguration DODataConfiguration)
		{
			var s3ClientConfig = new AmazonS3Config { ServiceURL = DODataConfiguration.DOServiceURL };
			IAmazonS3 _awsS3Client = new AmazonS3Client(DODataConfiguration.DOAccessKey, DODataConfiguration.DOSecretAccessKey, s3ClientConfig);

			try
			{
				using (var newMemoryStream = new MemoryStream())
				{
					file.CopyTo(newMemoryStream);

					var fileTransferUtility = new TransferUtility(_awsS3Client);
					var uploadRequest = new TransferUtilityUploadRequest
					{
						InputStream = newMemoryStream,
						Key = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName),
						BucketName = DODataConfiguration.DOBucketName,
						ContentType = file.ContentType,
						CannedACL = S3CannedACL.Private
					};

					uploadRequest.Metadata.Add("sender", "Jan");
					uploadRequest.Metadata.Add("receiver", "Bert");
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

		public async Task<bool> FindFileAsync(string fileName, DigitalOceanDataConfiguration DODataConfiguration)
		{
			var s3ClientConfig = new AmazonS3Config { ServiceURL = DODataConfiguration.DOServiceURL };
			IAmazonS3 _awsS3Client = new AmazonS3Client(DODataConfiguration.DOAccessKey, DODataConfiguration.DOSecretAccessKey, s3ClientConfig);

            try
            {
                GetObjectRequest getRequest = new GetObjectRequest
                {
                    BucketName = DODataConfiguration.DOBucketName,
                    Key = fileName // Keys are the full filename, including the file extension.
				};
                var respone = await _awsS3Client.GetObjectAsync(getRequest);
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
	}
}
