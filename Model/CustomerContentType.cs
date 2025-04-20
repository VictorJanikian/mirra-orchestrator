namespace Mirra_Orchestrator.Model
{
    public class CustomerContentType : Entity
    {
        public Customer Customer { get; set; }
        public ContentType ContentType { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
