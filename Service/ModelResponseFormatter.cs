using Mirra_Orchestrator.Integration.Model.Request;
using Mirra_Orchestrator.Model;
using Mirra_Orchestrator.Service.Interfaces;
using System.Text.RegularExpressions;

namespace Mirra_Orchestrator.Service
{
    public class ModelResponseFormatter : IModelResponseFormatter
    {
        public WordpressBlogPost GetWordpressBlogPostFromModelResponse(string modelResponse)
        {
            int divisorIndex = modelResponse.IndexOf("---");
            var postTitle = modelResponse.Substring(0, divisorIndex);
            var postContent = modelResponse.Substring(divisorIndex + 3);
            modelResponse = removeBlogPostsCommonErrors(modelResponse);
            WordpressBlogPost wordpressBlogPost = new(postTitle, postContent);
            return wordpressBlogPost;
        }

        public async Task<string> replaceImageMarkupsByImageLinks(string modelResponse, List<ImageInsideContent> imageAttributes)
        {
            if (string.IsNullOrEmpty(modelResponse) || imageAttributes == null || !imageAttributes.Any())
                return modelResponse;

            var sortedImages = imageAttributes.OrderByDescending(img => img.IndexOnText).ToList();
            var result = modelResponse;

            foreach (var image in sortedImages)
            {
                var htmlImage = $@"
                                <figure>
                                    <img src=""{image.ImageUrl}"" alt=""{System.Net.WebUtility.HtmlEncode(image.ImageCaption)}"" />
                                    <figcaption><em>{System.Net.WebUtility.HtmlEncode(image.ImageCaption)}</em></figcaption>
                                </figure>";

                result = result.Remove(image.IndexOnText, image.MarkupLength);
                result = result.Insert(image.IndexOnText, htmlImage);
            }

            return result;
        }

        private string removeBlogPostsCommonErrors(string modelResponse)
        {
            modelResponse = removeSpecialCharactersFromImageCaptions(modelResponse);
            return modelResponse;
        }

        private string removeSpecialCharactersFromImageCaptions(string modelResponse)
        {
            if (string.IsNullOrEmpty(modelResponse))
                return modelResponse;

            // Pattern para encontrar [IMG: ... &&& ...]
            var pattern = @"\[IMG:\s*(.+?)\s*&&&\s*(.+?)\s*\]";

            return Regex.Replace(modelResponse, pattern, match =>
            {
                var description = match.Groups[1].Value.Trim();
                var caption = match.Groups[2].Value.Trim();

                // Remove asteriscos do início e fim da legenda
                caption = caption.Trim('*');
                // Remove underscores do início e fim da legenda
                caption = caption.Trim('_');

                return $"[IMG: {description} &&& {caption}]";
            });
        }

    }

}