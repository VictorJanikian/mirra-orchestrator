using Mirra_Orchestrator.Integration.Interfaces;
using Mirra_Orchestrator.Model;
using Mirra_Orchestrator.Repository.Interfaces;
using Mirra_Orchestrator.Service.Interfaces;

namespace Mirra_Orchestrator.Service
{
    public class OrchestrationService : IOrchestrationService
    {

        IWordpressIntegration _wordpressIntegration;
        IContentGenerationService _contentGenerationService;
        IContentRepository _contentRepository;
        public OrchestrationService(IWordpressIntegration wordpressIntegration,
            IContentGenerationService contentGenerationService,
            IContentRepository contentRepository)
        {
            _wordpressIntegration = wordpressIntegration;
            _contentGenerationService = contentGenerationService;
            _contentRepository = contentRepository;
        }

        public async Task PostContent(Customer customer, Model.ContentType contentType, Parameters parameters)
        {
            var configurations = getContentTypeConfiguration(customer, contentType);

            switch ((Enums.ContentType)contentType.Id)
            {
                case Enums.ContentType.WORDPRESS:
                    await saveWordPressPost(customer, contentType, parameters, configurations);
                    break;
            }
        }

        private async Task saveWordPressPost(Customer customer, ContentType contentType, Parameters parameters, CustomerContentTypeConfiguration configurations)
        {
            var blogPost = await generateBlogPost(contentType, parameters);
            var postLink = await sendBlogPostToWordpress(configurations, blogPost);
            var summary = await generateBlogSummary(contentType, blogPost.ToString());
            var content = new Content()
            {
                ContentUrl = postLink,
                ContentSummary = summary,
                ContentType = contentType,
                Customer = customer,
                Parameters = parameters
            };

            await saveContent(content);
        }

        private async Task<Integration.Model.Request.WordpressBlogPost> generateBlogPost(ContentType contentType, Parameters parameters)
        {
            return await _contentGenerationService.GenerateBlogPost(parameters, contentType.SystemPrompt, contentType.Prompt);
        }

        private async Task<string> sendBlogPostToWordpress(CustomerContentTypeConfiguration configurations, Integration.Model.Request.WordpressBlogPost blogPost)
        {
            return await _wordpressIntegration.SendBlogPostToWordpress(blogPost, configurations);
        }

        private async Task<string> generateBlogSummary(ContentType contentType, string blogPost)
        {
            return await _contentGenerationService.GenerateBlogPostSummary(blogPost, contentType.SummaryPrompt);
        }

        private CustomerContentTypeConfiguration getContentTypeConfiguration(Customer customer, ContentType contentType)
        {
            var customerContentType = customer
                .CustomerContentTypes
                .Where(type => type.ContentType.Id == (int)(Enums.ContentType)contentType.Id)
                .FirstOrDefault();

            if (customerContentType != null)
            {
                customerContentType.Customer = customer;
                customerContentType.ContentType = contentType;
            }

            return customerContentType;
        }

        private async Task saveContent(Content content)
        {
            await _contentRepository.Create(content);
        }
    }
}
