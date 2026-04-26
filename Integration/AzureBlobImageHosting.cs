using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Mirra_Orchestrator.Integration.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Mirra_Orchestrator.Integration
{
    /// <summary>Uploads generated JPEG bytes to a public Azure Blob container and returns the public URL (for Instagram Graph <c>image_url</c>).</summary>
    public class AzureBlobImageHosting : IImageRepository
    {
        private readonly IConfiguration _configuration;

        public AzureBlobImageHosting(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> SaveImage(string url, string username, string password, byte[] image)
        {
            // WordPress-specific args are unused; all storage settings come from configuration.
            var connectionString = _configuration["AzureStorage:ConnectionString"]
                ?? throw new InvalidOperationException("AzureStorage:ConnectionString is not configured.");
            var containerName = _configuration["AzureStorage:ContainerName"]
                ?? throw new InvalidOperationException("AzureStorage:ContainerName is not configured.");

            var serviceClient = new BlobServiceClient(connectionString);
            var container = serviceClient.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobName = $"ig/{DateTime.UtcNow:yyyyMMdd}/{Guid.NewGuid():N}.jpg";
            var blob = container.GetBlobClient(blobName);

            await using var stream = new MemoryStream(image, writable: false);
            var headers = new BlobHttpHeaders { ContentType = "image/jpeg" };
            await blob.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = headers });

            return blob.Uri.AbsoluteUri;
        }
    }
}
