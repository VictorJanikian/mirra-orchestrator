using Mirra_Orchestrator.Enums;
using Mirra_Orchestrator.Exception;
using Mirra_Orchestrator.Integration.Interfaces;
using System.Net.Http.Headers;
using System.Text;

namespace Mirra_Orchestrator.Integration
{

    class RestClient : IRestClient
    {
        IHttpClientFactory _factory;

        public RestClient(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public async Task<HttpResponseMessage> post(string url, StringContent data, Dictionary<BasicAuthenticationParameter, string> authenticationParameters)
        {
            var client = _factory.CreateClient("wordpress");

            var username = authenticationParameters[BasicAuthenticationParameter.USERNAME];
            var password = authenticationParameters[BasicAuthenticationParameter.PASSWORD];

            //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
            setAuthorizationHeaders(client, username, password);

            var response = await client.PostAsync($"{url}", data);

            if (!response.IsSuccessStatusCode)
                throw new RestException(response.StatusCode.ToString());
            return response;


        }

        private void setAuthorizationHeaders(HttpClient client, string username, string password)
        {
            var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

    }
}
