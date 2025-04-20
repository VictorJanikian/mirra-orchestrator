namespace Mirra_Orchestrator.Model
{
    public class Customer : Entity
    {
        public string UniqueNumber { get; set; }
        public List<CustomerContentType> CustomerContentTypes { get; set; }
    }
}
