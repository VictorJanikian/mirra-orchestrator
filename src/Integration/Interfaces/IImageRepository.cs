namespace Mirra_Orchestrator.Integration.Interfaces
{
    public interface IImageRepository
    {
        public Task<string> SaveImage(string url, string username, string password, byte[] image);
    }
}
