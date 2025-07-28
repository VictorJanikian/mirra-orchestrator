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

        public WordpressIntegration(IRestClient restClient)
        {
            _restClient = restClient;
        }


        public async Task<string> SendBlogPostToWordpress(WordpressBlogPost blogPost, CustomerContentPlatformConfiguration configuration)
        {
            var authenticationParameters = new Dictionary<BasicAuthenticationParameter, string>()
            {
                {BasicAuthenticationParameter.USERNAME, configuration.Username },
                {BasicAuthenticationParameter.PASSWORD, configuration.Password }
            };

            using var wordpressResponse = await _restClient.post(configuration.Url, GetJSONFor(blogPost), authenticationParameters);

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
    }
}
