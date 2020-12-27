using System.Text.Json.Serialization;

namespace TFaller.Jsonball.Client.Events
{
    [JsonConverter(typeof(DocumentTypeRegistry.DocumentConverter))]
    public class Document
    {

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("version")]
        public uint Version { get; set; }

        [JsonPropertyName("doc")]
        public object Body { get; set; }
    }
}