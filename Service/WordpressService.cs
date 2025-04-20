using Mirra_Orchestrator.Enums;
using Mirra_Orchestrator.Integration.Interfaces;
using Mirra_Orchestrator.Integration.Model.Request;
using Mirra_Orchestrator.Model;
using Mirra_Orchestrator.Service.Interfaces;

namespace Mirra_Orchestrator.Service
{
    public class WordpressService : IWordpressService
    {
        IWordpressIntegration _wordpressIntegration;

        public WordpressService(IWordpressIntegration wordpressIntegration)
        {
            _wordpressIntegration = wordpressIntegration;
        }

        public async Task WriteBlogPost(Customer customer, Parameters parameters)
        {
            var customerWordpressConfiguration = customer.CustomerContentTypes.Where(type => type.ContentType.Id == (int)ContentTypes.WORDPRESS).FirstOrDefault();
            WordpressBlogPost blogPost = new WordpressBlogPost("Hello", "World");
            await _wordpressIntegration.WriteBlogPost(customerWordpressConfiguration.Url, blogPost, customerWordpressConfiguration.Username, customerWordpressConfiguration.Password);
        }
    }
}
