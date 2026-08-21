using Mirra_Orchestrator.Integration.Model.Response;

namespace Mirra_Orchestrator.Integration.Interfaces
{
    public interface IImageRepository
    {
        public Task<SavedImage> SaveImage(string url, string username, string password, byte[] image);
    }
}
