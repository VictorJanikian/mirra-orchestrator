namespace Mirra_Orchestrator.Integration.Interfaces
{
    public interface IImageRepository
    {
        public Task<string> SaveImage(byte[] image);
    }
}
