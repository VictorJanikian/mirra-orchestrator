using Mirra_Orchestrator.Exception;
using Mirra_Orchestrator.Integration.Model.Request;
using Mirra_Orchestrator.Model;
using Mirra_Orchestrator.Service.Interfaces;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Mirra_Orchestrator.Service
{
    public class ModelResponseFormatter : IModelResponseFormatter
    {
        public WordpressBlogPostRequest GetWordpressBlogPostFromModelResponse(string modelResponse)
        {
            var titleMatch = Regex.Match(modelResponse, @"<h1>(.*?)</h1>", RegexOptions.Singleline);
            var postTitle = titleMatch.Value;
            var postContent = modelResponse.Substring(titleMatch.Index + titleMatch.Length);
            postContent = removeBlogPostsCommonErrors(postContent);
            WordpressBlogPostRequest wordpressBlogPost = new(postTitle, postContent);
            return wordpressBlogPost;
        }

        public InstagramPost GetInstagramPostFromModelResponse(string modelResponse)
        {
            var instagramPost = JsonConvert.DeserializeObject<InstagramPost>(getJsonFromModelResponse(modelResponse));

            if (instagramPost == null || string.IsNullOrWhiteSpace(instagramPost.ImageDescription))
                throw new ModelResponseException("A resposta do modelo não contém a descrição da imagem do post.");

            return instagramPost;
        }

        private string getJsonFromModelResponse(string modelResponse)
        {
            var jsonMatch = Regex.Match(modelResponse ?? string.Empty, @"\{[\s\S]*\}");

            if (!jsonMatch.Success)
                throw new ModelResponseException("A resposta do modelo não contém um JSON.");

            return jsonMatch.Value;
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
                                    <img src=""{image.ImageUrl}"" alt=""{System.Net.WebUtility.HtmlEncode(image.ImageAlt)}"" />
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
            modelResponse = removeItalicTagsWrappingEntireParagraphs(modelResponse);
            modelResponse = includeLineBreakAfterImageMarkups(modelResponse);
            return modelResponse;
        }

        internal string removeItalicTagsWrappingEntireParagraphs(string modelResponse)
        {
            if (string.IsNullOrEmpty(modelResponse))
                return modelResponse;

            var pattern = @"<p([^>]*)>\s*<i>([\s\S]*?)</i>\s*</p>";
            return Regex.Replace(modelResponse, pattern, match =>
            {
                var paragraphAttributes = match.Groups[1].Value;
                var paragraphContent = match.Groups[2].Value;
                return $"<p{paragraphAttributes}>{paragraphContent}</p>";
            });
        }

        internal string removeSpecialCharactersFromImageCaptions(string modelResponse)
        {
            if (string.IsNullOrEmpty(modelResponse))
                return modelResponse;

            // Pattern para encontrar [IMG: ... &&& ... &&& ...]
            var pattern = @"\[IMG:\s*(.+?)\s*&&&\s*(.+?)\s*&&&\s*(.+?)\s*\]";

            return Regex.Replace(modelResponse, pattern, match =>
            {
                var description = match.Groups[1].Value.Trim();
                var caption = match.Groups[2].Value.Trim();
                var alt = match.Groups[3].Value.Trim();

                // Remove asteriscos do início e fim da legenda
                caption = caption.Trim('*');
                // Remove underscores do início e fim da legenda
                caption = caption.Trim('_');
                // Remove <i> do início
                caption = Regex.Replace(caption, @"^<i>\s*", "", RegexOptions.IgnoreCase);
                // Remove </i> do fim
                caption = Regex.Replace(caption, @"\s*</i>$", "", RegexOptions.IgnoreCase);

                return $"[IMG: {description} &&& {caption} &&& {alt}]";
            });
        }

        // A quebra de linha separa a imagem do parágrafo seguinte
        internal string includeLineBreakAfterImageMarkups(string modelResponse)
        {
            if (string.IsNullOrEmpty(modelResponse))
                return modelResponse;

            var pattern = @"\[IMG:\s*(.+?)\s*&&&\s*(.+?)\s*&&&\s*(.+?)\s*\]";
            return Regex.Replace(modelResponse, pattern, match => $"{match.Value}<br>");
        }

    }

}