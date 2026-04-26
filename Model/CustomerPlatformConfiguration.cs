namespace Mirra_Orchestrator.Model
{
    public class CustomerPlatformConfiguration : Entity
    {
        public Customer Customer { get; set; } = null!;
        public Platform Platform { get; set; } = null!;
        public string? Url { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? AccessToken { get; set; }
        public string? ExternalAccountId { get; set; }
    }
}
