using Mirra_Orchestrator.Integration;
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
        IInstagramIntegration _instagramIntegration;
        AzureBlobImageHosting _azureBlobImageHosting;
        IContentGenerationService _contentGenerationService;
        IContentRepository _contentRepository;
        IPreviousContentRecoveryService _previousContentRecoveryService;
        public OrchestrationService(IWordpressIntegration wordpressIntegration,
            IInstagramIntegration instagramIntegration,
            AzureBlobImageHosting azureBlobImageHosting,
            IContentGenerationService contentGenerationService,
            IContentRepository contentRepository,
            IPreviousContentRecoveryService previousContentRecoveryService)
        {
            _wordpressIntegration = wordpressIntegration;
            _instagramIntegration = instagramIntegration;
            _azureBlobImageHosting = azureBlobImageHosting;
            _contentGenerationService = contentGenerationService;
            _contentRepository = contentRepository;
            _previousContentRecoveryService = previousContentRecoveryService;
        }

        public async Task PostContent(Scheduling schedule, Customer customer, Platform platform, Parameters parameters)
        {

            switch ((Enums.Platform)platform.Id)
            {
                case Enums.Platform.WORDPRESS:
                    await saveWordPressPost(customer, platform, parameters, schedule.CustomerPlatformConfiguration);
                    break;
                case Enums.Platform.INSTAGRAM:
                    await saveInstagramPost(customer, platform, parameters, schedule.CustomerPlatformConfiguration);
                    break;
            }
        }


        private async Task saveWordPressPost(Customer customer, Platform platform, Parameters parameters, CustomerPlatformConfiguration configurations)
        {
            List<Content> lastPosts = await getLastsPostsForThis(configurations);
            var blogPost = await generateBlogPost(configurations, parameters, lastPosts);
            var postLink = await sendBlogPostToWordpress(configurations, blogPost);
            var summary = await generateBlogSummary(platform, blogPost.ToString());
            var content = new Content()
            {
                ContentTitle = RemoveHtmlTags(blogPost.title),
                ContentUrl = postLink,
                ContentSummary = summary,
                CustomerPlatformConfiguration = configurations,
                Parameters = parameters
            };

            await saveContent(content);
        }

        private async Task saveInstagramPost(Customer customer, Platform platform, Parameters parameters, CustomerPlatformConfiguration configurations)
        {
            List<Content> lastPosts = await getLastsPostsForThis(configurations);
            var post = await _contentGenerationService.GenerateInstagramPost(parameters, configurations, lastPosts, _azureBlobImageHosting);
            var mediaId = await _instagramIntegration.PublishPhotoPost(configurations, post);
            var summary = await generateBlogSummary(platform, post.Caption);
            var content = new Content()
            {
                ContentTitle = InstagramContentTitleFromCaption(post.Caption),
                ContentUrl = mediaId,
                ContentSummary = summary,
                CustomerPlatformConfiguration = configurations,
                Parameters = parameters
            };
            await saveContent(content);
        }

        private static string InstagramContentTitleFromCaption(string caption)
        {
            if (string.IsNullOrWhiteSpace(caption)) return "(Instagram post)";
            var firstLine = caption.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .FirstOrDefault() ?? caption.Trim();
            return firstLine.Length > 200 ? firstLine[..200] : firstLine;
        }

        private async Task<List<Content>> getLastsPostsForThis(CustomerPlatformConfiguration configurations)
        {
            return await _previousContentRecoveryService.getLastContentsFrom(configurations);
        }

        private async Task<Integration.Model.Request.WordpressBlogPost> generateBlogPost(CustomerPlatformConfiguration configuration, Parameters parameters, List<Content> lastPosts)
        {
            return await _contentGenerationService.GenerateBlogPost(parameters, configuration, lastPosts, _wordpressIntegration);
        }

        private async Task<string> sendBlogPostToWordpress(CustomerPlatformConfiguration configurations, Integration.Model.Request.WordpressBlogPost blogPost)
        {
            return await _wordpressIntegration.SendBlogPostToWordpress(configurations, blogPost);
        }

        private async Task<string> generateBlogSummary(Platform platform, string blogPost)
        {
            return await _contentGenerationService.GenerateBlogPostSummary(blogPost, platform.SummaryPrompt);
        }


        private async Task saveContent(Content content)
        {
            await _contentRepository.Create(content);
        }
    }
}
