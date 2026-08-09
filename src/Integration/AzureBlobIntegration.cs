using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Mirra_Orchestrator.Exception;
using Mirra_Orchestrator.Integration.Interfaces;
using System.Text;

namespace Mirra_Orchestrator.Integration
{
    public class AzureBlobIntegration : IAzureBlobIntegration
    {
        private readonly IConfiguration _configuration;

        public AzureBlobIntegration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> SaveImage(string fileName, byte[] image)
        {
            using var imageStream = new MemoryStream(image);
            return await uploadBlob(fileName, imageStream, "image/png");
        }

        public async Task<string> SaveText(string fileName, string text)
        {
            using var textStream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            return await uploadBlob(fileName, textStream, "text/plain");
        }

        private async Task<string> uploadBlob(string fileName, Stream content, string contentType)
        {
            var container = getContainerClient();
            await container.CreateIfNotExistsAsync();

            var blob = container.GetBlobClient(fileName);
            var uploadOptions = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
            };

            await blob.UploadAsync(content, uploadOptions);

            return blob.Uri.ToString();
        }

        private BlobContainerClient getContainerClient()
        {
            var serviceUrl = getRequiredConfiguration("Storage:AzureBlob:Url");
            var accountName = getRequiredConfiguration("Storage:AzureBlob:AccountName");
            var accessKey = getRequiredConfiguration("Storage:AzureBlob:AccessKey");
            var containerName = getRequiredConfiguration("Storage:AzureBlob:ContainerName");

            var credential = new StorageSharedKeyCredential(accountName, accessKey);
            var serviceClient = new BlobServiceClient(new Uri(serviceUrl), credential);

            return serviceClient.GetBlobContainerClient(containerName);
        }

        private string getRequiredConfiguration(string key)
        {
            var value = _configuration[key];

            if (string.IsNullOrWhiteSpace(value))
                throw new AzureBlobException($"A configuração '{key}' não foi informada.");

            return value;
        }
    }
}
