namespace Mirra_Orchestrator.Integration.Model.Response
{
    public class SavedImage
    {
        public SavedImage(int id, string url)
        {
            Id = id;
            Url = url;
        }

        public int Id { get; set; }

        public string Url { get; set; }
    }
}
