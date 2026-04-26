namespace Mirra_Orchestrator.Exception
{
    public class InstagramException : System.Exception
    {
        public InstagramException()
        {
        }

        public InstagramException(string? message) : base(message)
        {
        }

        public InstagramException(string? message, System.Exception inner) : base(message, inner)
        {
        }
    }
}
