using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using FileMicroservice.DTOs;
using FileMicroservice.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMicroservice.IntegrationTests.Stubs
{
    public class LocalstackFileProvider : IFileProvider
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
				await _awsS3Client.GetObjectAsync(getRequest);
				return new Dictionary<bool, string>() { { true, "12" } };
			}
			catch (Exception exception)
			{
				if (exception.InnerException != null)
				{
					if (exception.Message.Contains("NoSuchBucket"))
					{
						return new Dictionary<bool, string>() { { false, string.Empty } };
					}

					else if (exception.Message.Contains("NotFound"))
					{
						return new Dictionary<bool, string>() { { false, string.Empty } };
					}
				}
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

					uploadRequest.Metadata.Add("senderid", fileDto.SenderId);
					uploadRequest.Metadata.Add("receiverid", fileDto.ReceiverId);
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

		internal IAmazonS3 CreateAWSS3Client(DigitalOceanDataConfigDTO DODataConfigDto)
		{
			var s3ClientConfig = new AmazonS3Config { ServiceURL = DODataConfigDto.DOServiceURL, ForcePathStyle = true };
			IAmazonS3 _awsS3Client = new AmazonS3Client(DODataConfigDto.DOAccessKey, DODataConfigDto.DOSecretAccessKey, s3ClientConfig);
			return _awsS3Client;
		}
	}
}
