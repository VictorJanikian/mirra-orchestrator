using Mirra_Orchestrator.Integration.Model.Request;
using Mirra_Orchestrator.Model;

namespace Mirra_Orchestrator.Integration.Interfaces
{
    public interface IInstagramIntegration
    {
        /// <summary>Publishes a single photo. Returns the published media id from Graph API.</summary>
        Task<string> PublishPhotoPost(CustomerPlatformConfiguration config, InstagramPost post);
    }
}
