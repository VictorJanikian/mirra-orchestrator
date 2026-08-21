using Newtonsoft.Json;

namespace Mirra_Orchestrator.Integration.Model.Request
{
    public class WordpressBlogPostRequest
    {
        public WordpressBlogPostRequest(string title, string content)
        {
            this.title = title;
            this.content = content;
        }

        public string title { get; set; }

        public string content { get; set; }

        public string status { get; set; } = "publish";

        // Imagem de capa (destacada) do post - id do anexo em /wp/v2/media
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? featured_media { get; set; }

        public override string ToString()
        {
            return "Title: " + title + " Content: " + content;
        }
    }
}
