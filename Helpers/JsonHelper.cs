using Newtonsoft.Json;
using System.Text;

namespace Mirra_Orchestrator.Helpers
{
    public static class JsonHelper
    {
        public static StringContent GetJSONFor(object item)
        {

            return new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
        }
    }
}
