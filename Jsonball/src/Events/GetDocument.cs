using System;
using System.Text.Json.Serialization;

namespace TFaller.Jsonball.Client.Events
{
    public class GetDocument : IEquatable<GetDocument>
    {

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is GetDocument)
            {
                return Equals((GetDocument)obj);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode() + Name.GetHashCode();
        }

        public bool Equals(GetDocument other)
        {
            return Type == other.Type
                && Name == other.Name;
        }
    }
}