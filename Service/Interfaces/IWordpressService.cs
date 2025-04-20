using Mirra_Orchestrator.Model;

namespace Mirra_Orchestrator.Service.Interfaces
{
    public interface IWordpressService
    {
        public Task WriteBlogPost(Customer customer, Parameters parameters);
    }
}
