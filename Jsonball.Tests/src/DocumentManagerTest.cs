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
    }

    [DocumentType("test-doc-type")]
    internal class SimpleDocument
    { }
}