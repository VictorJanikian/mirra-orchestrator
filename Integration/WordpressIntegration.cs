using Mirra_Orchestrator.Enums;
using Mirra_Orchestrator.Exception;
using Mirra_Orchestrator.Integration.Interfaces;
using Mirra_Orchestrator.Integration.Model.Request;
using Mirra_Orchestrator.Model;
using System.Text.Json;
using static Mirra_Orchestrator.Helpers.JsonHelper;
namespace Mirra_Orchestrator.Integration
{
    class WordpressIntegration : IWordpressIntegration
    {
        private readonly IRestClient _restClient;

        public CustomerPlatformTableRow _configuration { get; set; }

        public WordpressIntegration(IRestClient restClient)
        {
            _restClient = restClient;
        }

        public async Task<string> SendBlogPostToWordpress(WordpressBlogPost blogPost)
        {
            var authenticationParameters = new Dictionary<BasicAuthenticationParameter, string>()
            {
                {BasicAuthenticationParameter.USERNAME, _configuration.Username },
                {BasicAuthenticationParameter.PASSWORD, _configuration.Password }
            };

            using var wordpressResponse = await _restClient.post(_configuration.Url + "/wp/v2/posts", GetJSONFor(blogPost), authenticationParameters);

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


        public async Task<string> SaveImage(byte[] image)
        {
            var authenticationParameters = new Dictionary<BasicAuthenticationParameter, string>()
            {
                {BasicAuthenticationParameter.USERNAME, _configuration.Username },
                {BasicAuthenticationParameter.PASSWORD, _configuration.Password }
            };

            var endpoint = $"{_configuration.Url.TrimEnd('/')}/wp/v2/media";

            using var content = new ByteArrayContent(image);

            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            {
                FileName = $"image_{DateTime.UtcNow:yyyyMMddHHmmss}.jpg"
            };

            using var wordpressResponse = await _restClient.post(endpoint, content, authenticationParameters);

            return await getMediaUrlFromResponse(wordpressResponse);
        }

        private async Task<string> getMediaUrlFromResponse(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            var responseStream = await response.Content.ReadAsStreamAsync();
            using var responseJson = await JsonDocument.ParseAsync(responseStream);

            // Retorna a URL da imagem
            if (responseJson.RootElement.TryGetProperty("source_url", out JsonElement urlElement))
                return urlElement.GetString()!;

            throw new WordpressException("A resposta não contém o atributo 'source_url'.");
        }

    }
}
