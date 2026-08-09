namespace Mirra_Orchestrator.Exception
{
    public class AzureBlobException : System.Exception
    {
        public AzureBlobException()
        {
        }

        public AzureBlobException(string? message) : base(message)
        {
        }
    }
}
