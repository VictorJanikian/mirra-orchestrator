using Mirra_Orchestrator.Integration.Model.Request;
using Mirra_Orchestrator.Model;
using Mirra_Orchestrator.Service.Interfaces;

namespace Mirra_Orchestrator.Service
{
    public class ContentGenerationService : IContentGenerationService
    {
        IPromptFormatterService _promptFormatterService;
        IModelCommunicationService _modelCommunicationService;
        IModelResponseFormatter _modelResponseFormatter;

        public ContentGenerationService(IPromptFormatterService promptFormatterService,
             IModelCommunicationService modelCommunicationService,
             IModelResponseFormatter modelResponseFormatter)
        {
            _promptFormatterService = promptFormatterService;
            _modelCommunicationService = modelCommunicationService;
            _modelResponseFormatter = modelResponseFormatter;
        }

        public async Task<WordpressBlogPost> GenerateBlogPost(Parameters parameters, string? systemPrompt,
            string prompt, List<Content> lastPosts)
        {
            var formattedPrompt = await _promptFormatterService.ReplacePromptVariables(prompt, parameters, lastPosts);
            var modelResponse = await _modelCommunicationService.GetTextResponse(systemPrompt, formattedPrompt);
            var postRetrievedFromModelResponse = _modelResponseFormatter.GetWordpressBlogPostFromModelResponse(modelResponse.ToString());
            return postRetrievedFromModelResponse;

        }

        public async Task<string> GenerateBlogPostSummary(string originalPost, string summaryPrompt)
        {
            var formattedPrompt = await _promptFormatterService.ReplaceTextInsidePrompt(summaryPrompt, originalPost);
            var modelResponse = await _modelCommunicationService.GetTextResponse(string.Empty, formattedPrompt);
            return modelResponse.ToString();
        }
    }
}
