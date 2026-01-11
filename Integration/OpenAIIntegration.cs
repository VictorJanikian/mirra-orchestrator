using Microsoft.Extensions.Configuration;
using Mirra_Orchestrator.Integration.Interfaces;
using OpenAI.Images;

namespace Mirra_Orchestrator.Integration
{
    public class OpenAIIntegration : IOpenAIIntegration
    {
        private IConfiguration _configuration;
        private HttpClient _httpClient;

        public OpenAIIntegration(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<byte[]> GenerateImage(string prompt)
        {

            var apiKey = _configuration["AI:OpenAI:ApiKey"];

            ImageClient client = new("gpt-image-1-mini", apiKey);

            ImageGenerationOptions options = new()
            {
                Quality = GeneratedImageQuality.Medium,
                Size = GeneratedImageSize.W1024xH1024
            };

            GeneratedImage image = await client.GenerateImageAsync(prompt, options);
            return image.ImageBytes.ToArray();
        }
    }
}
