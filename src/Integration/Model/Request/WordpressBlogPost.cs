namespace Mirra_Orchestrator.Integration.Model.Request
{
    public class WordpressBlogPost
    {
        public WordpressBlogPost(string title, string content)
        {
            this.Title = title;
            this.Content = content;
        }

        public string Title { get; set; }

        public string Content { get; set; }

        public string Status { get; set; } = "publish";

        public override string ToString()
        {
            return "Title: " + Title + " Content: " + Content;
        }
    }
}
