namespace AutoPartInventorySystem.Services.Contracts
{
    public interface IStorageService
    {
        Task<MemoryStream> GetObjectAsync(string bucketName, string objectName);
        Task DeleteObjectAsync(string bucketName, string objectName);
        Task<bool> AddObjectAsync(string bucketName, string objectName, Stream stream);
    }
}
