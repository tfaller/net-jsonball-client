using System.Text.Json.Serialization;

namespace TFaller.Jsonball.Client.Events
{
    public class GetDocument
    {

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}