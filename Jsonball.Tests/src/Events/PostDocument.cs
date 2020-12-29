using System.Text.Json;
using TFaller.Jsonball.Client.Events;
using TFaller.Jsonball.Tests.Client.Tracing;
using Xunit;


namespace TFaller.Jsonball.Tests.Client.Events
{
    public class PostDocumentTest
    {
        [Fact]
        public void TestSerialization()
        {
            PostDocument<IPerson> postDoc = new PostDocument<IPerson>()
            {
                Type = "person",
                Name = "someone",
                Document = new Person()
                {
                    Name = "firstname",
                }
            };

            var doc = JsonSerializer.Serialize(postDoc);
            Assert.Equal("{\"type\":\"person\",\"name\":\"someone\",\"doc\":{\"name\":\"firstname\",\"Parent\":null,\"Parents\":null}}", doc);
        }
    }
}