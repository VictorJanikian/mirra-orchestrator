namespace Mirra_Orchestrator.Exception
{
    class RestException : System.Exception
    {
        public RestException()
        {
        }

        public RestException(string? message) : base(message)
        {
        }
    }
}
