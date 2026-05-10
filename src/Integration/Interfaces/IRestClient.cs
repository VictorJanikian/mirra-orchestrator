using Mirra_Orchestrator.Enums;

namespace Mirra_Orchestrator.Integration.Interfaces
{
    public interface IRestClient
    {
        Task<HttpResponseMessage> post(string url, HttpContent data, Dictionary<BasicAuthenticationParameter, string> authenticationParameters);
    }
}
