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

        public async Task<WordpressBlogPost> GenerateBlogPost(Parameters parameters,
            string prompt)
        {
            var formattedPrompt = await _promptFormatterService.ReplacePromptVariables(prompt, parameters);
            var modelResponse = await _modelCommunicationService.GetTextResponse(formattedPrompt);
            var postRetrievedFromModelResponse = _modelResponseFormatter.GetWordpressBlogPostFromModelResponse(modelResponse.ToString());
            return postRetrievedFromModelResponse;

        }

        public async Task<string> GenerateBlogPostSummary(string originalPost, string summaryPrompt)
        {
            var formattedPrompt = await _promptFormatterService.ReplaceTextInsidePrompt(summaryPrompt, originalPost);
            var modelResponse = await _modelCommunicationService.GetTextResponse(formattedPrompt);
            return modelResponse.ToString();
        }
    }
}
