using Amazon.S3;
using Amazon.S3.Model;
using AutoPartInventorySystem.Services.Contracts;

namespace AutoPartInventorySystem.Services.Implementations
{
    public class S3StorageService : IStorageService
    {
        public IAmazonS3 Client;

        public S3StorageService(IAmazonS3 client)
        {
            Client = client;
        }
        public async Task<bool> AddObjectAsync(string bucketName, string objectName, Stream stream)
        {
            try
            {
                var request = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectName,
                    InputStream = stream
                };

                await Client.PutObjectAsync(request);
                return true;
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Could not upload {objectName} to {bucketName}: '{ex.Message}'");
                return false;
            }
        }

        public async Task DeleteObjectAsync(string bucketName, string objectName)
        {
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectName,
                };

                await Client.DeleteObjectAsync(deleteObjectRequest);
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error encountered on server. Message:'{ex.Message}' when deleting an object.");
            }
        }

        public async Task<MemoryStream> GetObjectAsync(string bucketName, string objectName)
        {
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectName
                };

                using GetObjectResponse response = await Client.GetObjectAsync(request);
                var memoryStream = new MemoryStream();
                await response.ResponseStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                return memoryStream;
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error getting object from S3: {ex.Message}");
                throw;
            }
        }
    }
}
