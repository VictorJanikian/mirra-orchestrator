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

        public async Task<WordpressBlogPost> GenerateBlogPost(Parameters parameters, string? systemPrompt,
            string prompt, List<Content> lastPosts, IImageRepository imageRepository)
        {
            var formattedPrompt = await _promptFormatterService.ReplacePromptVariables(prompt, parameters, lastPosts);
            var modelResponse = await _modelCommunicationService.GetTextResponse(systemPrompt, formattedPrompt);
            modelResponse = await includeImages(modelResponse, imageRepository);
            var postRetrievedFromModelResponse = _modelResponseFormatter.GetWordpressBlogPostFromModelResponse(modelResponse.ToString());
            return postRetrievedFromModelResponse;

        }

        private async Task<string> includeImages(string modelResponse, IImageRepository imageRepository)
        {
            var imagesAttributes = recoverListOfImagesToBeGenerated(modelResponse);
            foreach (var imageAttributes in imagesAttributes)
            {
                byte[] image = await generateImage(imageAttributes);
                await saveImage(imageRepository, imageAttributes, image);
            }
            return await _modelResponseFormatter.replaceImageMarkupsByImageLinks(modelResponse, imagesAttributes);

        }

        private async Task saveImage(IImageRepository imageRepository, ImageInsideContent imageAttributes, byte[] image)
        {
            imageAttributes.ImageUrl = await imageRepository.SaveImage(image);
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

        public async Task<string> GenerateBlogPostSummary(string originalPost, string summaryPrompt)
        {
            var formattedPrompt = await _promptFormatterService.ReplaceTextInsidePrompt(summaryPrompt, originalPost);
            var modelResponse = await _modelCommunicationService.GetTextResponse(string.Empty, formattedPrompt);
            return modelResponse.ToString();
        }
    }
}
