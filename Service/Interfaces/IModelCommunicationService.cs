namespace Mirra_Orchestrator.Service.Interfaces
{
    public interface IModelCommunicationService
    {
        public Task<string> GetTextResponse(string? systemPrompt, string prompt);

        public Task<byte[]> GetImageResponse(string prompt);
    }
}
