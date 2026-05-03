using Mirra_Orchestrator.Model;

namespace Mirra_Orchestrator.Service.Interfaces
{
    public interface IModelCommunicationService
    {
        public Task<string> GetTextResponse(string? systemPrompt, string prompt, ConversationMetadata metadata);

        public Task<byte[]> GetImageResponse(string prompt);
    }
}
