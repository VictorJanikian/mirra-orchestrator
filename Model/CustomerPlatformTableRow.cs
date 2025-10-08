namespace Mirra_Orchestrator.Model
{
    public class CustomerPlatformTableRow : Entity
    {
        public Customer Customer { get; set; }
        public Platform Platform { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
