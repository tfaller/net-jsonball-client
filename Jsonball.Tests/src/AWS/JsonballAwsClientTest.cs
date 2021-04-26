using Amazon.Lambda;
using Amazon.Lambda.Model;
using Moq;
using System.IO;
using System.Text;
using System.Threading;
using TFaller.Jsonball.Client;
using TFaller.Jsonball.Client.AWS;
using Xunit;

namespace TFaller.Jsonball.Tests.Client.AWS
{
    public class JsonballAwsClientTest
    {
        [Fact]
        public async void TestNotExistGetDocumentAsync()
        {
            var targetFunction = "lambda:function";

            // setup
            DocumentTypeRegistry.RegisterDocumentType<BasicEmptyInterface>("empty-interface");

            var lambdaMock = new Mock<IAmazonLambda>();
            lambdaMock.Setup(l => l.InvokeAsync(
                It.IsAny<InvokeRequest>(),
                It.IsAny<CancellationToken>())
            ).ReturnsAsync(
                new InvokeResponse()
                {
                    Payload = new MemoryStream(Encoding.UTF8.GetBytes(
                        @"{""type"":""empty-interface"",""doc"":null}"
                    ))
                }
            );

            var client = new JsonballAwsClient(lambdaMock.Object, null);
            client.GetDocumentFunction = targetFunction;

            // performe test

            var doc = await client.GetDocumentAsync(null);

            Assert.Null(doc.Body);
            Assert.Equal<uint>(0, doc.Version);
        }
    }

    interface BasicEmptyInterface { }
}