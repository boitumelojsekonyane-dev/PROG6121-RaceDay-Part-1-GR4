
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RaceDay.API.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobStorageService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureStorage:ConnectionString"];
            var containerName = configuration["AzureStorage:ContainerName"];

            if (string.IsNullOrWhiteSpace(connectionString) ||
                string.IsNullOrWhiteSpace(containerName))
            {
                throw new InvalidOperationException(
                    "Azure Storage configuration is missing.");
            }

            _containerClient = new BlobContainerClient(
                connectionString, containerName);
        }

        public async Task<string> UploadImageAsync(
            Stream imageStream,
            string fileName,
            string contentType)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();
            string blobName = $"{Guid.NewGuid():N}{extension}";

            await _containerClient.CreateIfNotExistsAsync(
                PublicAccessType.None);

            BlobClient blobClient = _containerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(
                imageStream,
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = contentType
                    }
                });

            return blobName;
        }

        public async Task<(Stream Content, string ContentType)> DownloadImageAsync(
            string blobName)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(blobName);
            var response = await blobClient.DownloadStreamingAsync();

            return (
                response.Value.Content,
                response.Value.Details.ContentType ?? "application/octet-stream"
            );
        }
    }
}
