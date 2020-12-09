using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TFaller.Jsonball.Client.Events;

namespace TFaller.Jsonball.Client.AWS
{
    public class Client : Jsonball.Client.Client
    {
        private AmazonLambdaClient _lambda;
        private AmazonSQSClient _sqs;
        public string PostDocumentQueueUrl { get; set; }
        public string ListenOnChangeQueueUrl { get; set; }
        public string GetDocumentFunction { get; set; }

        public Client(AmazonLambdaClient lambda, AmazonSQSClient sqs)
        {
            _lambda = lambda;
            _sqs = sqs;
        }

        public override async Task PostDocumentAsync(PostDocument doc, CancellationToken ct = default)
        {
            var request = new SendMessageRequest();
            request.QueueUrl = PostDocumentQueueUrl;
            request.MessageBody = JsonSerializer.Serialize(doc);
            request.MessageGroupId = CreateDocName(doc.Type, doc.Name);
            await _sqs.SendMessageAsync(request, ct);
        }

        public override async Task<Document> GetDocumentAsync(GetDocument doc, CancellationToken ct = default)
        {
            var request = new InvokeRequest();
            request.InvocationType = InvocationType.RequestResponse;
            request.FunctionName = GetDocumentFunction;
            request.Payload = JsonSerializer.Serialize(doc);
            var response = await _lambda.InvokeAsync(request, ct);
            return await JsonSerializer.DeserializeAsync<Document>(response.Payload, cancellationToken: ct);
        }

        public override async Task ListenOnChange(ListenOnChange listen, CancellationToken ct = default)
        {
            var request = new SendMessageRequest();
            request.QueueUrl = ListenOnChangeQueueUrl;
            request.MessageBody = JsonSerializer.Serialize(listen);
            await _sqs.SendMessageAsync(request, ct);
        }
    }
}