using Mirra_Orchestrator.Enums;
using Mirra_Orchestrator.Integration.Interfaces;
using Mirra_Orchestrator.Integration.Model.Request;
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

        public async Task WriteBlogPost(string url, WordpressBlogPost blogPost, string user, string password)
        {
            var authenticationParameters = new Dictionary<BasicAuthenticationParameter, string>()
            {
                {BasicAuthenticationParameter.USERNAME, user },
                {BasicAuthenticationParameter.PASSWORD, password }
            };

            await _restClient.post(url, GetJSONFor(blogPost), authenticationParameters);
        }
    }
}
