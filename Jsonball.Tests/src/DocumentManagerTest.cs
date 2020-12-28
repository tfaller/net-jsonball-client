using Moq;
using System.Threading;
using TFaller.Jsonball.Client;
using TFaller.Jsonball.Client.Events;
using Xunit;

namespace TFaller.Jsonball.Tests.Client
{
    public class DocumentManagerTest
    {

        [Fact]
        public async void TestDocumentTypeAttribute()
        {
            var getDoc = new GetDocument()
            {
                Type = "test-doc-type",
                Name = "test"
            };

            var returnDoc = new Document()
            {
                Type = "test-doc-type",
                Name = "test",
                Body = new SimpleDocument()
            };

            var jbClient = new Moq.Mock<JsonballClient>();
            jbClient.Setup(c => c.GetDocumentAsync(getDoc, CancellationToken.None)).ReturnsAsync(returnDoc);

            var dm = new DocumentManager(jbClient.Object);
            var doc = await dm.GetDocumentAsync<SimpleDocument>("test", CancellationToken.None);

            Assert.Equal<SimpleDocument>((SimpleDocument)returnDoc.Body, doc);
        }

        [Fact]
        public async void TestTracing()
        {
            var getDoc = new GetDocument()
            {
                Type = "person",
                Name = "firstname",
            };

            var jbClient = new Mock<JsonballClient>();
            jbClient.Setup(c => c.GetDocumentAsync(getDoc, It.IsAny<CancellationToken>())).ReturnsAsync(
                new Document()
                {
                    Type = "person",
                    Name = "firstname",
                    Version = 5,
                    Body = new Tracing.Person()
                    {
                        Name = "firstname"
                    }
                });

            var dm = new DocumentManager(jbClient.Object, true);
            var preson = await dm.GetDocumentAsync<Tracing.IPerson>("person", "firstname");

            // with the following assert wie use the property "name"
            // it should be traced ...
            Assert.Equal("firstname", preson.Name);

            var listener = dm.BuildListenOnChange();

            Assert.Equal(1, listener.Length);
            Assert.Equal("person", listener[0].Type);
            Assert.Equal("firstname", listener[0].Name);
            Assert.Equal<uint>(5, listener[0].Version);
            Assert.Equal(new string[] { "/name" }, listener[0].Properties);
        }
    }

    [DocumentType("test-doc-type")]
    internal class SimpleDocument
    { }
}