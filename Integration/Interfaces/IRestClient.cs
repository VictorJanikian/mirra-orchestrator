using Mirra_Orchestrator.Enums;

namespace Mirra_Orchestrator.Integration.Interfaces
{
    public interface IRestClient
    {
        Task post(string url, StringContent data, Dictionary<BasicAuthenticationParameter, string> authenticationParameters);
    }
}
