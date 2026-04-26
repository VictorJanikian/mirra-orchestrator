namespace Mirra_Orchestrator.Integration.Model.Request
{
    /// <summary>Parsed model output before image generation and blob upload.</summary>
    public class InstagramModelParseResult
    {
        public string ImageDescription { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
    }
}
