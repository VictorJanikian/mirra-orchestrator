using AutoMapper;
using Microsoft.SemanticKernel.ChatCompletion;
using Mirra_Orchestrator.Integration.Interfaces;
using Mirra_Orchestrator.Model;
using Mirra_Orchestrator.Repository.Interfaces;
using Mirra_Orchestrator.Service.Interfaces;

namespace Mirra_Orchestrator.Service
{
    public class ModelCommunicationService : IModelCommunicationService
    {
        private readonly IChatCompletionService _chatService;
        private readonly IOpenAIIntegration _openAIIntegration;
        private readonly IModelConversationHistoryRepository _conversationHistoryRepository;
        private readonly IMapper _mapper;

        public ModelCommunicationService(
            IChatCompletionService chatService,
            IOpenAIIntegration openAIIntegration,
            IModelConversationHistoryRepository modelConversationHistoryRepository,
            IMapper mapper)
        {
            _chatService = chatService;
            _openAIIntegration = openAIIntegration;
            _conversationHistoryRepository = modelConversationHistoryRepository;
            _mapper = mapper;
        }

        public async Task<string> GetTextResponse(string? systemPrompt, string prompt, ConversationMetadata metadata)
        {
            ChatHistory chat = new();
            if (!string.IsNullOrEmpty(systemPrompt))
                chat.AddSystemMessage(systemPrompt);
            chat.AddUserMessage(prompt);
            var modelResponse = await _chatService.GetChatMessageContentAsync(chat);
            var modelResponseString = modelResponse.ToString();
            ModelConversationHistory conversationHistory = new ModelConversationHistory
            {
                SystemPrompt = systemPrompt,
                Prompt = prompt,
                ModelResponse = modelResponseString
            };
            _mapper.Map(metadata, conversationHistory);

            await _conversationHistoryRepository.Create(conversationHistory);

            return modelResponseString;
        }

        public async Task<byte[]> GetImageResponse(string prompt)
        {
            var imageBytes = await _openAIIntegration.GenerateImage(prompt);
            return imageBytes;
        }
    }
}