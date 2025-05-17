namespace Mirra_Orchestrator.Service.Interfaces
{
    public interface IModelCommunicationService
    {
        public Task<string> GetTextResponse(string prompt);
    }
}
