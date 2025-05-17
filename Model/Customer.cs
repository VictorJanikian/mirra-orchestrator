namespace Mirra_Orchestrator.Model
{
    public class Customer : Entity
    {
        public string UniqueNumber { get; set; }
        public List<CustomerContentTypeConfiguration> CustomerContentTypes { get; set; }
    }
}
