using Microsoft.SemanticKernel.ChatCompletion;
using Mirra_Orchestrator.Service.Interfaces;

namespace Mirra_Orchestrator.Service
{
    public class ModelCommunicationService : IModelCommunicationService
    {
        IChatCompletionService _chatService;

        public ModelCommunicationService(IChatCompletionService chatService)
        {
            _chatService = chatService;
        }

        public async Task<string> GetTextResponse(string prompt)
        {
            ChatHistory chat = new();
            chat.AddUserMessage(prompt);
            var modelResponse = await _chatService.GetChatMessageContentAsync(chat);
            return modelResponse.ToString();
        }
    }
}
