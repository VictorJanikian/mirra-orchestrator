namespace Mirra_Orchestrator.Integration.Model.Request
{
    public class WordpressBlogPost
    {
        public WordpressBlogPost(string title, string content)
        {
            this.title = title;
            this.content = content;
        }

        public string title { get; set; }

        public string content { get; set; }

        public string status { get; set; } = "publish";

        public override string ToString()
        {
            return "Title: " + title + " Content: " + content;
        }
    }
}
