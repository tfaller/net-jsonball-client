using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace TFaller.Jsonball.Client.Events
{
    public class ListenOnChangeDocument : IEquatable<ListenOnChangeDocument>
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("newDoc")]
        public bool NewDocument { get; set; }

        [JsonPropertyName("version")]
        public uint Version { get; set; }

        [JsonPropertyName("props")]
        public string[] Properties { get; set; }

        public bool Equals(ListenOnChangeDocument other)
        {
            return Type == other.Type
                && Name == other.Name
                && NewDocument == other.NewDocument
                && Version == other.Version
                && Properties.SequenceEqual(other.Properties);
        }
    }
}