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
            var wordpressPostRequest = _modelResponseFormatter.GetWordpressBlogPostFromModelResponse(modelResponse.ToString());
            await includeImages(platformConfiguration, wordpressPostRequest, imageRepository);
            return wordpressPostRequest;

        }

        private async Task includeImages(CustomerPlatformConfiguration platformConfiguration, WordpressBlogPostRequest blogPost, IImageRepository imageRepository)
        {
            var imagesAttributes = recoverListOfImagesToBeGenerated(blogPost.content);
            var featuredImage = getFeaturedImage(imagesAttributes);

            foreach (var imageAttributes in imagesAttributes)
            {
                byte[] image = await generateImage(imageAttributes.ImageDescription);
                await saveImage(imageRepository, imageAttributes, image, platformConfiguration, imageAttributes == featuredImage);
            }

            blogPost.featured_media = featuredImage?.ImageId;
            var imagesInsideBody = removeFeaturedImageMarkup(blogPost, featuredImage, imagesAttributes);
            blogPost.content = await _modelResponseFormatter.replaceImageMarkupsByImageLinks(blogPost.content, imagesInsideBody);
        }

        // A imagem de capa do post e a primeira imagem gerada para o conteudo
        private ImageInsideContent? getFeaturedImage(List<ImageInsideContent> imagesAttributes)
        {
            return imagesAttributes.OrderBy(image => image.IndexOnText).FirstOrDefault();
        }

        // A capa nao se repete no corpo do post: o markup dela e removido e os indices
        // das demais imagens, todas posteriores, sao recuados pelo tamanho removido
        private List<ImageInsideContent> removeFeaturedImageMarkup(WordpressBlogPostRequest blogPost, ImageInsideContent? featuredImage, List<ImageInsideContent> imagesAttributes)
        {
            if (featuredImage == null)
                return imagesAttributes;

            var removedLength = featuredImage.MarkupLength + getLineBreakLengthAfterMarkup(blogPost.content, featuredImage);
            blogPost.content = blogPost.content.Remove(featuredImage.IndexOnText, removedLength);

            var imagesInsideBody = imagesAttributes.Where(image => image != featuredImage).ToList();
            foreach (var image in imagesInsideBody)
                image.IndexOnText -= removedLength;

            return imagesInsideBody;
        }

        private int getLineBreakLengthAfterMarkup(string content, ImageInsideContent image)
        {
            var lineBreak = "<br>";
            var indexAfterMarkup = image.IndexOnText + image.MarkupLength;
            var contentAfterMarkup = content.Substring(indexAfterMarkup);
            return contentAfterMarkup.StartsWith(lineBreak) ? lineBreak.Length : 0;
        }

        private async Task saveImage(IImageRepository imageRepository, ImageInsideContent imageAttributes, byte[] image, CustomerPlatformConfiguration platformConfiguration, bool isFeaturedImage)
        {
            // A capa nao leva legenda: ela seria exibida pelo tema do WordPress logo abaixo da imagem destacada
            var caption = isFeaturedImage ? null : imageAttributes.ImageCaption;
            var savedImage = await imageRepository.SaveImage(platformConfiguration.Url, platformConfiguration.Username, platformConfiguration.Password, image, imageAttributes.ImageAlt, caption);
            imageAttributes.ImageUrl = savedImage.Url;
            imageAttributes.ImageId = savedImage.Id;
        }

        private async Task<byte[]> generateImage(string imageDescription)
        {
            return await _modelCommunicationService.GetImageResponse(imageDescription);
        }

        private List<ImageInsideContent> recoverListOfImagesToBeGenerated(string modelResponse)
        {
            var imagesList = new List<ImageInsideContent>();

            if (string.IsNullOrEmpty(modelResponse))
                return imagesList;

            var matches = findImagesMarkupOnModelResponse(modelResponse);

            foreach (Match match in matches)
            {
                if (match.Success && match.Groups.Count == 4)
                {
                    string imageDescription = match.Groups[1].Value.Trim();
                    string imageCaption = match.Groups[2].Value.Trim();
                    string imageAlt = match.Groups[3].Value.Trim();
                    int indexOnText = match.Index;
                    int markupLength = match.Length;

                    imagesList.Add(new ImageInsideContent
                    {
                        ImageDescription = imageDescription,
                        ImageCaption = imageCaption,
                        ImageAlt = imageAlt,
                        IndexOnText = indexOnText,
                        MarkupLength = markupLength
                    });
                }
            }

            return imagesList;
        }

        private MatchCollection findImagesMarkupOnModelResponse(string modelResponse)
        {
            // Pattern: [IMG: {description} &&& {subtitle} &&& {alt}]
            var pattern = @"\[IMG:\s*(.+?)\s*&&&\s*(.+?)\s*&&&\s*(.+?)\s*\]";
            return Regex.Matches(modelResponse, pattern);
        }

        public async Task<string> GenerateBlogPostSummary(int schedulingId, string originalPost, string summaryPrompt)
        {
            var formattedPrompt = await _promptFormatterService.ReplaceTextInsidePrompt(summaryPrompt, originalPost);
            ConversationMetadata metadata = new ConversationMetadata { SchedulingId = schedulingId };
            var modelResponse = await _modelCommunicationService.GetTextResponse(string.Empty, formattedPrompt, metadata);
            return modelResponse.ToString();
        }

        public async Task<InstagramPost> GenerateInstagramSinglePost(int schedulingId, Parameters parameters, ContentType contentType, List<Content> lastPosts)
        {
            var instagramPost = await generateInstagramPostDescription(schedulingId, parameters, contentType, lastPosts);
            instagramPost.Image = await generateImage(instagramPost.ImageDescription);
            return instagramPost;
        }

        public Task<InstagramPost> GenerateInstagramCarrousselPost(int schedulingId, Parameters parameters, ContentType contentType, List<Content> lastPosts)
        {
            throw new NotImplementedException();
        }

        private async Task<InstagramPost> generateInstagramPostDescription(int schedulingId, Parameters parameters, ContentType contentType, List<Content> lastPosts)
        {
            var formattedPrompt = await _promptFormatterService.ReplacePromptVariables(contentType.Prompt, parameters, lastPosts);
            ConversationMetadata metadata = new ConversationMetadata { SchedulingId = schedulingId };
            var modelResponse = await _modelCommunicationService.GetTextResponse(contentType.SystemPrompt, formattedPrompt, metadata);
            return _modelResponseFormatter.GetInstagramPostFromModelResponse(modelResponse.ToString());
        }
    }
}
