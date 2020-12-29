using System.Text.Json.Serialization;

namespace TFaller.Jsonball.Client.Events
{
    /// <summary>
    /// PostDocument is used to post a new or updated document to
    /// the jsonball service.
    /// </summary>
    /// <typeparam name="TDoc">The type of the document</typeparam>
    public sealed class PostDocument<TDoc>
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("doc")]
        public TDoc Document { get; set; }
    }
}
