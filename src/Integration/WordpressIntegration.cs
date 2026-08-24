using Mirra_Orchestrator.Enums;
using Mirra_Orchestrator.Exception;
using Mirra_Orchestrator.Helpers;
using Mirra_Orchestrator.Integration.Interfaces;
using Mirra_Orchestrator.Integration.Model.Request;
using Mirra_Orchestrator.Integration.Model.Response;
using Mirra_Orchestrator.Model;
using System.Text;
using System.Text.Json;
using static Mirra_Orchestrator.Helpers.JsonHelper;
namespace Mirra_Orchestrator.Integration
{
    class WordpressIntegration : IWordpressIntegration
    {
        private readonly IRestClient _restClient;
        private readonly SymmetricEncryptionHelper _symmetricEncryptionHelper;

        public WordpressIntegration(IRestClient restClient, SymmetricEncryptionHelper symmetricEncryptionHelper)
        {
            _restClient = restClient;
            _symmetricEncryptionHelper = symmetricEncryptionHelper;
        }

        public async Task<string> SendBlogPostToWordpress(CustomerPlatformConfiguration platformConfiguration, WordpressBlogPostRequest blogPost)
        {
            var authenticationParameters = new Dictionary<BasicAuthenticationParameter, string>()
            {
                {BasicAuthenticationParameter.USERNAME, platformConfiguration.Username },
                {BasicAuthenticationParameter.PASSWORD, _symmetricEncryptionHelper.Decrypt(platformConfiguration.Password) }
            };

            using var wordpressResponse = await _restClient.post(platformConfiguration.Url + "/wp/v2/posts", GetJSONFor(blogPost), authenticationParameters);

            return await getPostLinkFromResponse(wordpressResponse);

        }

        private async Task<string> getPostLinkFromResponse(HttpResponseMessage response)
        {

            var responseStream = await response.Content.ReadAsStreamAsync();
            using var responseJson = await JsonDocument.ParseAsync(responseStream);

            if (responseJson.RootElement.TryGetProperty("link", out JsonElement linkElement))
                return linkElement.GetString()!;

            else
                throw new WordpressException("A resposta não contém o atributo 'link'.");
        }


        public async Task<SavedImage> SaveImage(string url, string username, string password, byte[] image, string altText, string? caption)
        {
            var authenticationParameters = new Dictionary<BasicAuthenticationParameter, string>()
            {
                {BasicAuthenticationParameter.USERNAME, username },
                {BasicAuthenticationParameter.PASSWORD, _symmetricEncryptionHelper.Decrypt(password) }
            };

            var endpoint = $"{url.TrimEnd('/')}/wp/v2/media";

            using var content = buildImageUploadContent(image, altText, caption);

            using var wordpressResponse = await _restClient.post(endpoint, content, authenticationParameters);
            return await getSavedImageFromResponse(wordpressResponse);

        }

        // O alt e a legenda pertencem ao anexo, entao vao no mesmo multipart do arquivo
        private MultipartFormDataContent buildImageUploadContent(byte[] image, string altText, string? caption)
        {
            var content = new MultipartFormDataContent();

            var imageContent = new ByteArrayContent(image);
            imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

            content.Add(imageContent, "file", $"image_{DateTime.UtcNow:yyyyMMddHHmmss}.jpg");
            content.Add(new StringContent(altText, Encoding.UTF8), "alt_text");

            if (!string.IsNullOrWhiteSpace(caption))
                content.Add(new StringContent(caption, Encoding.UTF8), "caption");

            return content;
        }

        private async Task<SavedImage> getSavedImageFromResponse(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            var responseStream = await response.Content.ReadAsStreamAsync();
            using var responseJson = await JsonDocument.ParseAsync(responseStream);

            // Retorna o id e a URL da imagem
            if (!responseJson.RootElement.TryGetProperty("source_url", out JsonElement urlElement))
                throw new WordpressException("A resposta não contém o atributo 'source_url'.");

            if (!responseJson.RootElement.TryGetProperty("id", out JsonElement idElement))
                throw new WordpressException("A resposta não contém o atributo 'id'.");

            return new SavedImage(idElement.GetInt32(), urlElement.GetString()!);
        }

    }
}
