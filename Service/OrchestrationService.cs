using Mirra_Orchestrator.Integration.Interfaces;
using Mirra_Orchestrator.Model;
using Mirra_Orchestrator.Repository.Interfaces;
using Mirra_Orchestrator.Service.Interfaces;
using static Mirra_Orchestrator.Helpers.TextHelper;

namespace Mirra_Orchestrator.Service
{
    public class OrchestrationService : IOrchestrationService
    {

        IWordpressIntegration _wordpressIntegration;
        IContentGenerationService _contentGenerationService;
        IContentRepository _contentRepository;
        IPreviousContentRecoveryService _previousContentRecoveryService;
        public OrchestrationService(IWordpressIntegration wordpressIntegration,
            IContentGenerationService contentGenerationService,
            IContentRepository contentRepository,
            IPreviousContentRecoveryService previousContentRecoveryService)
        {
            _wordpressIntegration = wordpressIntegration;
            _contentGenerationService = contentGenerationService;
            _contentRepository = contentRepository;
            _previousContentRecoveryService = previousContentRecoveryService;
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
            List<Content> lastPosts = await getLastsPostsFrom(customer, contentType);
            var blogPost = await generateBlogPost(contentType, parameters, lastPosts);
            var postLink = await sendBlogPostToWordpress(configurations, blogPost);
            var summary = await generateBlogSummary(contentType, blogPost.ToString());
            var content = new Content()
            {
                ContentTitle = RemoveHtmlTags(blogPost.title),
                ContentUrl = postLink,
                ContentSummary = summary,
                ContentType = contentType,
                Customer = customer,
                Parameters = parameters
            };

            await saveContent(content);
        }

        private async Task<List<Content>> getLastsPostsFrom(Customer customer, ContentType contentType)
        {
            return await _previousContentRecoveryService.getLastContentsFrom(customer, contentType);
        }

        private async Task<Integration.Model.Request.WordpressBlogPost> generateBlogPost(ContentType contentType, Parameters parameters, List<Content> lastPosts)
        {
            return await _contentGenerationService.GenerateBlogPost(parameters, contentType.SystemPrompt, contentType.Prompt, lastPosts);
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
