namespace Mirra_Orchestrator.Model
{
    public class Customer : Entity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public List<CustomerPlatformTableRow> CustomerPlatforms { get; set; }
    }
}
