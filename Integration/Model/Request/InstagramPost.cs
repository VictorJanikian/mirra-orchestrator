namespace Mirra_Orchestrator.Integration.Model.Request
{
    /// <summary>Payload to publish a single image post to Instagram (Graph API).</summary>
    public class InstagramPost
    {
        public InstagramPost(string imageUrl, string caption)
        {
            ImageUrl = imageUrl;
            Caption = caption;
        }

        public string ImageUrl { get; set; }
        public string Caption { get; set; }
    }
}
