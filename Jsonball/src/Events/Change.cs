using System.Text.Json.Serialization;

namespace TFaller.Jsonball.Client.Events
{
    public class Change
    {
        [JsonPropertyName("handler")]
        public string Hander { get; set; }

        [JsonPropertyName("docs")]
        public Document[] Documents { get; set; }
    }
}