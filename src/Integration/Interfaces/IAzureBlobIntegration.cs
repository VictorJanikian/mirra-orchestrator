namespace Mirra_Orchestrator.Integration.Interfaces
{
    public interface IAzureBlobIntegration
    {
        Task<string> SaveImage(string fileName, byte[] image);
        Task<string> SaveText(string fileName, string text);
    }
}
