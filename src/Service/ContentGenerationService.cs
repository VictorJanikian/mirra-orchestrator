using Mirra_Orchestrator.Integration.Interfaces;
using Mirra_Orchestrator.Integration.Model.Request;
using Mirra_Orchestrator.Model;
using Mirra_Orchestrator.Service.Interfaces;
using System.Text.RegularExpressions;

namespace Mirra_Orchestrator.Service
{
    public class ContentGenerationService : IContentGenerationService
    {
        IPromptFormatterService _promptFormatterService;
        IModelCommunicationService _modelCommunicationService;
        IModelResponseFormatter _modelResponseFormatter;

        public ContentGenerationService(IPromptFormatterService promptFormatterService,
             IModelCommunicationService modelCommunicationService,
             IModelResponseFormatter modelResponseFormatter)
        {
            _promptFormatterService = promptFormatterService;
            _modelCommunicationService = modelCommunicationService;
            _modelResponseFormatter = modelResponseFormatter;
        }

        public async Task<WordpressBlogPostRequest> GenerateBlogPost(int schedulingId, Parameters parameters, CustomerPlatformConfiguration platformConfiguration, ContentType contentType, List<Content> lastPosts, IImageRepository imageRepository)
        {
            var prompt = contentType.Prompt;
            var systemPrompt = contentType.SystemPrompt;
            var formattedPrompt = await _promptFormatterService.ReplacePromptVariables(prompt, parameters, lastPosts);
            ConversationMetadata metadata = new ConversationMetadata { SchedulingId = schedulingId };
            var modelResponse = await _modelCommunicationService.GetTextResponse(systemPrompt, formattedPrompt, metadata);
            var postRetrievedFromModelResponse = _modelResponseFormatter.GetWordpressBlogPostFromModelResponse(modelResponse.ToString());
            postRetrievedFromModelResponse.content = await includeImages(platformConfiguration, postRetrievedFromModelResponse.content, imageRepository);
            return postRetrievedFromModelResponse;

        }

        private async Task<string> includeImages(CustomerPlatformConfiguration platformConfiguration, string modelResponse, IImageRepository imageRepository)
        {
            var imagesAttributes = recoverListOfImagesToBeGenerated(modelResponse);
            foreach (var imageAttributes in imagesAttributes)
            {
                byte[] image = await generateImage(imageAttributes);
                await saveImage(imageRepository, imageAttributes, image, platformConfiguration);
            }
            return await _modelResponseFormatter.replaceImageMarkupsByImageLinks(modelResponse, imagesAttributes);

        }

        private async Task saveImage(IImageRepository imageRepository, ImageInsideContent imageAttributes, byte[] image, CustomerPlatformConfiguration platformConfiguration)
        {
            imageAttributes.ImageUrl = await imageRepository.SaveImage(platformConfiguration.Url, platformConfiguration.Username, platformConfiguration.Password, image);
        }

        private async Task<byte[]> generateImage(ImageInsideContent imageAttributes)
        {
            return await _modelCommunicationService.GetImageResponse(imageAttributes.ImageDescription);
        }

        private List<ImageInsideContent> recoverListOfImagesToBeGenerated(string modelResponse)
        {
            var imagesList = new List<ImageInsideContent>();

            if (string.IsNullOrEmpty(modelResponse))
                return imagesList;

            var matches = findImagesMarkupOnModelResponse(modelResponse);

            foreach (Match match in matches)
            {
                if (match.Success && match.Groups.Count == 3)
                {
                    string imageDescription = match.Groups[1].Value.Trim();
                    string imageCaption = match.Groups[2].Value.Trim();
                    int indexOnText = match.Index;
                    int markupLength = match.Length;

                    imagesList.Add(new ImageInsideContent
                    {
                        ImageDescription = imageDescription,
                        ImageCaption = imageCaption,
                        IndexOnText = indexOnText,
                        MarkupLength = markupLength
                    });
                }
            }

            return imagesList;
        }

        private MatchCollection findImagesMarkupOnModelResponse(string modelResponse)
        {
            // Pattern: [IMG: {description} &&& {subtitle}]
            var pattern = @"\[IMG:\s*(.+?)\s*&&&\s*(.+?)\s*\]";
            return Regex.Matches(modelResponse, pattern);
        }

        public async Task<string> GenerateBlogPostSummary(int schedulingId, string originalPost, string summaryPrompt)
        {
            var formattedPrompt = await _promptFormatterService.ReplaceTextInsidePrompt(summaryPrompt, originalPost);
            ConversationMetadata metadata = new ConversationMetadata { SchedulingId = schedulingId };
            var modelResponse = await _modelCommunicationService.GetTextResponse(string.Empty, formattedPrompt, metadata);
            return modelResponse.ToString();
        }
    }
}
