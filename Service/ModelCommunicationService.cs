using Microsoft.SemanticKernel.ChatCompletion;
using Mirra_Orchestrator.Integration.Interfaces;
using Mirra_Orchestrator.Service.Interfaces;

namespace Mirra_Orchestrator.Service
{
    public class ModelCommunicationService : IModelCommunicationService
    {
        private readonly IChatCompletionService _chatService;
        private readonly IOpenAIIntegration _openAIIntegration;

        public ModelCommunicationService(
            IChatCompletionService chatService,
            IOpenAIIntegration openAIIntegration)
        {
            _chatService = chatService;
            _openAIIntegration = openAIIntegration;
        }

        public async Task<string> GetTextResponse(string? systemPrompt, string prompt)
        {
            ChatHistory chat = new();
            if (!string.IsNullOrEmpty(systemPrompt))
                chat.AddSystemMessage(systemPrompt);
            chat.AddUserMessage(prompt);
            var modelResponse = await _chatService.GetChatMessageContentAsync(chat);
            return modelResponse.ToString();
        }

        public async Task<byte[]> GetImageResponse(string prompt)
        {
            var imageBytes = await _openAIIntegration.GenerateImage(prompt);
            return imageBytes;
        }
    }
}