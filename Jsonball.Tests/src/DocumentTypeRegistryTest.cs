using System.Text.Json;
using System.Text.Json.Serialization;
using TFaller.Jsonball.Client;
using TFaller.Jsonball.Client.Events;
using Xunit;

namespace TFaller.Jsonball.Tests.Client
{
    public class DocumentTypeRegistryTest
    {

        [Fact]
        public void TestCustomDocumentType()
        {
            DocumentTypeRegistry.RegisterDocumentType<TestDocument>("person");

            var doc = JsonSerializer.Deserialize<Document>(
                "{\"name\": \"some-name\", \"version\": 1, \"type\": \"person\", \"doc\": {\"name\": \"firstname\", \"age\": 123}}"
            );

            Assert.Equal("person", doc.Type);
            Assert.Equal("some-name", doc.Name);
            Assert.Equal<uint>(1, doc.Version);

            var testDoc = (TestDocument)doc.Body;
            Assert.Equal("firstname", testDoc.Name);
            Assert.Equal(123, testDoc.Age);
        }

        [Fact]
        public void TestInvalidProperty()
        {
            var ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Document>(@"{""foo"":""bar""}"));
            Assert.Equal("Invalid document property: foo", ex.Message);
        }

        [Fact]
        public void TestNotObject()
        {
            var ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Document>(@"true"));
            Assert.StartsWith("Expected an object, but got ", ex.Message);
        }

        [Fact]
        public void TestUnknownDocType()
        {
            var ex = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Document>(@"{""type"":""hello"",""doc"":null}"));
            Assert.Equal("Unknown document type: hello", ex.Message);
        }
    }

    public class TestDocument
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("age")]
        public int Age { get; set; }
    }
}
