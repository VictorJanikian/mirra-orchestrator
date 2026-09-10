using Mirra_Orchestrator.Enums;
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
        IAzureBlobIntegration _azureBlobIntegration;
        IContentGenerationService _contentGenerationService;
        IContentRepository _contentRepository;
        IPreviousContentRecoveryService _previousContentRecoveryService;
        public OrchestrationService(IWordpressIntegration wordpressIntegration,
            IInstagramIntegration instagramIntegration,
            IAzureBlobIntegration azureBlobIntegration,
            IContentGenerationService contentGenerationService,
            IContentRepository contentRepository,
            IPreviousContentRecoveryService previousContentRecoveryService)
        {
            _wordpressIntegration = wordpressIntegration;
            _instagramIntegration = instagramIntegration;
            _azureBlobIntegration = azureBlobIntegration;
            _contentGenerationService = contentGenerationService;
            _contentRepository = contentRepository;
            _previousContentRecoveryService = previousContentRecoveryService;
        }

        public async Task PostContent(Scheduling schedule, Customer customer, Platform platform, Parameters parameters)
        {

            switch ((Enums.EPlatform)platform.Id)
            {
                case Enums.EPlatform.WORDPRESS:
                    await saveWordPressPost(schedule, customer, platform, parameters, schedule.CustomerPlatformConfiguration);
                    break;
                case Enums.EPlatform.INSTAGRAM:
                    await saveInstagramPost(schedule, customer, platform, parameters, schedule.CustomerPlatformConfiguration);
                    break;
            }
        }


        private async Task saveWordPressPost(Scheduling schedule, Customer customer, Platform platform, Parameters parameters, CustomerPlatformConfiguration configuration)
        {
            List<Content> lastPosts = await getLastsPostsForThis(configuration);
            var blogPost = await generateBlogPost(schedule, configuration, parameters, lastPosts);
            var postLink = await sendBlogPostToWordpress(configuration, blogPost);
            var summary = await generateBlogSummary(schedule, blogPost.ToString());
            var content = new Content()
            {
                ContentTitle = RemoveHtmlTags(blogPost.title),
                ContentUrl = postLink,
                ContentSummary = summary,
                CustomerPlatformConfiguration = configuration,
                Parameters = parameters
            };

            await saveContent(content);
        }

        private async Task saveInstagramPost(Scheduling schedule, Customer customer, Platform platform, Parameters parameters, CustomerPlatformConfiguration configuration)
        {
            List<Content> lastPosts = await getLastsPostsForThis(configuration);
            var instagramPost = await generateInstagramPost(schedule, parameters, lastPosts);
            var savedImage = await sendInstagramPostToBlobStorage(schedule, instagramPost);
            await publishInstagramPost(configuration, savedImage.FileName, instagramPost.Caption);

            var content = new Content()
            {
                ContentTitle = parameters.ThemeTitle,
                ContentUrl = savedImage.Url,
                ContentSummary = instagramPost.ImageDescription,
                CustomerPlatformConfiguration = configuration,
                Parameters = parameters
            };

            await saveContent(content);
        }

        private async Task<InstagramPost> generateInstagramPost(Scheduling schedule, Parameters parameters, List<Content> lastPosts)
        {
            switch ((EContentType)schedule.ContentType.Id)
            {
                case EContentType.INSTAGRAM_SINGLE_POST:
                    return await _contentGenerationService.GenerateInstagramSinglePost(schedule.Id, parameters, schedule.ContentType, lastPosts);
                case EContentType.INSTAGRAM_CARROUSEL_POST:
                    return await _contentGenerationService.GenerateInstagramCarrousselPost(schedule.Id, parameters, schedule.ContentType, lastPosts);
                default:
                    throw new NotImplementedException();
            }

        }

        private async Task<(string FileName, string Url)> sendInstagramPostToBlobStorage(Scheduling schedule, InstagramPost instagramPost)
        {
            var fileName = buildInstagramPostFileName(schedule);
            var imageFileName = $"{fileName}.png";

            await _azureBlobIntegration.SaveText($"{fileName}.txt", instagramPost.Caption);
            var imageUrl = await _azureBlobIntegration.SaveImage(imageFileName, instagramPost.Image);

            return (imageFileName, imageUrl);
        }

        // A Graph API baixa a imagem por conta propria, entao o container privado precisa expor um link assinado
        private async Task publishInstagramPost(CustomerPlatformConfiguration configuration, string imageFileName, string caption)
        {
            var temporaryImageUrl = _azureBlobIntegration.GenerateTemporaryReadUrl(imageFileName, TimeSpan.FromHours(1));
            await _instagramIntegration.PublishImagePost(configuration, temporaryImageUrl, caption);
        }

        private string buildInstagramPostFileName(Scheduling schedule)
        {
            return $"instagram/{schedule.Id}/post_{DateTime.UtcNow:yyyyMMddHHmmss}";
        }

        private async Task<List<Content>> getLastsPostsForThis(CustomerPlatformConfiguration configuration)
        {
            return await _previousContentRecoveryService.getLastContentsFrom(configuration);
        }

        private async Task<Integration.Model.Request.WordpressBlogPostRequest> generateBlogPost(Scheduling schedule, CustomerPlatformConfiguration configuration, Parameters parameters, List<Content> lastPosts)
        {
            return await _contentGenerationService.GenerateBlogPost(schedule.Id, parameters, configuration, schedule.ContentType, lastPosts, _wordpressIntegration);
        }

        private async Task<string> sendBlogPostToWordpress(CustomerPlatformConfiguration configurations, Integration.Model.Request.WordpressBlogPostRequest blogPost)
        {
            return await _wordpressIntegration.SendBlogPostToWordpress(configurations, blogPost);
        }

        private async Task<string> generateBlogSummary(Scheduling schedule, string blogPost)
        {
            return await _contentGenerationService.GenerateBlogPostSummary(schedule.Id, blogPost, schedule.ContentType.SummaryPrompt);
        }


        private async Task saveContent(Content content)
        {
            await _contentRepository.Create(content);
        }
    }
}
