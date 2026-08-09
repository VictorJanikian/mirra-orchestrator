using Newtonsoft.Json;

namespace Mirra_Orchestrator.Model
{
    public class InstagramPost
    {
        [JsonProperty("image_description")]
        public string ImageDescription { get; set; }

        [JsonProperty("caption")]
        public string Caption { get; set; }

        [JsonIgnore]
        public byte[] Image { get; set; }

        public override string ToString()
        {
            return "Image description: " + ImageDescription + " Caption: " + Caption;
        }
    }
}
