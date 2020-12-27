using System.Text.Json.Serialization;

namespace TFaller.Jsonball.Client.Events
{
    public class ListenOnChange
    {
        [JsonPropertyName("handler")]
        public string Handler { get; set; }

        [JsonPropertyName("docs")]
        public ListenOnChangeDocument[] Documents { get; set; }
    }
}