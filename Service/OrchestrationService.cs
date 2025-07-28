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

        public async Task PostContent(Customer customer, Model.ContentPlatform contentPlatform, Parameters parameters)
        {
            var configurations = getContentPlatformConfiguration(customer, contentPlatform);

            switch ((Enums.ContentPlatform)contentPlatform.Id)
            {
                case Enums.ContentPlatform.WORDPRESS:
                    await saveWordPressPost(customer, contentPlatform, parameters, configurations);
                    break;
            }
        }

        private async Task saveWordPressPost(Customer customer, ContentPlatform contentPlatform, Parameters parameters, CustomerContentPlatformConfiguration configurations)
        {
            List<Content> lastPosts = await getLastsPostsForThis(configurations);
            var blogPost = await generateBlogPost(contentPlatform, parameters, lastPosts);
            var postLink = await sendBlogPostToWordpress(configurations, blogPost);
            var summary = await generateBlogSummary(contentPlatform, blogPost.ToString());
            var content = new Content()
            {
                ContentTitle = RemoveHtmlTags(blogPost.title),
                ContentUrl = postLink,
                ContentSummary = summary,
                CustomerContentPlatformConfiguration = configurations,
                Parameters = parameters
            };

            await saveContent(content);
        }

        private async Task<List<Content>> getLastsPostsForThis(CustomerContentPlatformConfiguration configurations)
        {
            return await _previousContentRecoveryService.getLastContentsFrom(configurations);
        }

        private async Task<Integration.Model.Request.WordpressBlogPost> generateBlogPost(ContentPlatform contentPlatform, Parameters parameters, List<Content> lastPosts)
        {
            return await _contentGenerationService.GenerateBlogPost(parameters, contentPlatform.SystemPrompt, contentPlatform.Prompt, lastPosts);
        }

        private async Task<string> sendBlogPostToWordpress(CustomerContentPlatformConfiguration configurations, Integration.Model.Request.WordpressBlogPost blogPost)
        {
            return await _wordpressIntegration.SendBlogPostToWordpress(blogPost, configurations);
        }

        private async Task<string> generateBlogSummary(ContentPlatform contentPlatform, string blogPost)
        {
            return await _contentGenerationService.GenerateBlogPostSummary(blogPost, contentPlatform.SummaryPrompt);
        }

        private CustomerContentPlatformConfiguration getContentPlatformConfiguration(Customer customer, ContentPlatform contentPlatform)
        {
            var customerContentPlatform = customer
                .CustomerContentPlatforms
                .Where(type => type.ContentPlatform.Id == (int)(Enums.ContentPlatform)contentPlatform.Id)
                .FirstOrDefault();

            if (customerContentPlatform != null)
            {
                customerContentPlatform.Customer = customer;
                customerContentPlatform.ContentPlatform = contentPlatform;
            }

            return customerContentPlatform;
        }

        private async Task saveContent(Content content)
        {
            await _contentRepository.Create(content);
        }
    }
}
