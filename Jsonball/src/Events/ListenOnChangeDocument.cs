using System.Text.Json.Serialization;

namespace TFaller.Jsonball.Client.Events
{
    public class ListenOnChangeDocument
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("version")]
        public uint Version { get; set; }

        [JsonPropertyName("props")]
        public string[] Properties { get; set; }
    }
}