using System.ComponentModel.DataAnnotations.Schema;

namespace Mirra_Orchestrator.Repository.DbEntities
{
    [Table("customer_platforms_configurations")]
    public class CustomerPlatformConfigurationTableRow : EntityTableRow
    {
        public int CustomerId { get; set; }
        public CustomerTableRow Customer { get; set; } = null!;
        public int PlatformId { get; set; }
        public PlatformTableRow Platform { get; set; } = null!;
        /// <summary>WordPress REST base URL, e.g. https://blog.example.com/wp-json</summary>
        public string? Url { get; set; }
        public string? Username { get; set; }
        /// <summary>Encrypted application password (WordPress) or other secrets.</summary>
        public string? Password { get; set; }
        /// <summary>Long-lived access token (Instagram Graph), encrypted (same as Password field).</summary>
        public string? AccessToken { get; set; }
        /// <summary>Instagram Business account id (IG_USER_ID) for Graph API.</summary>
        public string? ExternalAccountId { get; set; }
    }
}
