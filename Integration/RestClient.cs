using Mirra_Orchestrator.Enums;
using Mirra_Orchestrator.Exception;
using Mirra_Orchestrator.Integration.Interfaces;
using System.Net.Http.Headers;
using System.Text;

namespace Mirra_Orchestrator.Integration
{
    class RestClient : IRestClient
    {
        public async Task post(string url, StringContent data, Dictionary<BasicAuthenticationParameter, string> authenticationParameters)
        {
            using (var client = new HttpClient())
            {
                var username = authenticationParameters[BasicAuthenticationParameter.USERNAME];
                var password = authenticationParameters[BasicAuthenticationParameter.PASSWORD];

                //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
                setAuthorizationHeaders(client, username, password);

                using (var response = await client.PostAsync($"{url}", data))
                {
                    if (!response.IsSuccessStatusCode)
                        throw new RestException(response.StatusCode.ToString());
                }
            }
        }

        private void setAuthorizationHeaders(HttpClient client, string username, string password)
        {
            var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

    }
}
