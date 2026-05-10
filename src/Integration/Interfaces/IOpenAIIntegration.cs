namespace Mirra_Orchestrator.Integration.Interfaces
{
    public interface IOpenAIIntegration
    {
        public Task<byte[]> GenerateImage(string prompt);
    }
}
