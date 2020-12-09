using System.Text.Json.Serialization;

namespace TFaller.Jsonball.Client.Events
{
    public class PostDocument
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("doc")]
        public object Document { get; set; }
    }
}
