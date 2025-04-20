using Mirra_Orchestrator.Enums;
using Mirra_Orchestrator.Model;
using Mirra_Orchestrator.Service.Interfaces;

namespace Mirra_Orchestrator.Service
{
    public class OrchestrationService : IOrchestrationService
    {

        IWordpressService _wordpressService;

        public OrchestrationService(IWordpressService wordpressService)
        {
            _wordpressService = wordpressService;
        }

        public async Task PostContent(Customer customer, ContentType contentType, Parameters parameters)
        {
            switch ((ContentTypes)contentType.Id)
            {
                case ContentTypes.WORDPRESS:
                    await _wordpressService.WriteBlogPost(customer, parameters);
                    break;
            }
        }
    }
}
