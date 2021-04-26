using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TFaller.Jsonball.Client.Events;

namespace TFaller.Jsonball.Client.AWS
{
    public class JsonballAwsClient : JsonballClient
    {
        private IAmazonLambda _lambda;
        private AmazonSQSClient _sqs;
        private JsonSerializerOptions _jsonDeserializeOptions;
        public string PostDocumentQueueUrl { get; set; }
        public string ListenOnChangeQueueUrl { get; set; }
        public string ListenOnChangeFunction { get; set; }
        public string GetDocumentFunction { get; set; }

        public JsonballAwsClient(IAmazonLambda lambda, AmazonSQSClient sqs, JsonSerializerOptions jsonDeserializeOptions = null)
        {
            _lambda = lambda;
            _sqs = sqs;
            _jsonDeserializeOptions = jsonDeserializeOptions;
        }

        public override async Task PostDocumentAsync<T>(PostDocument<T> doc, CancellationToken ct = default)
        {
            var request = new SendMessageRequest();
            request.QueueUrl = PostDocumentQueueUrl;
            request.MessageBody = JsonSerializer.Serialize(doc);
            request.MessageGroupId = CreateDocName(doc.Type, doc.Name);
            request.MessageDeduplicationId = Guid.NewGuid().ToString();
            await _sqs.SendMessageAsync(request, ct);
        }

        public override async Task<Document> GetDocumentAsync(GetDocument doc, CancellationToken ct = default)
        {
            var request = new InvokeRequest();
            request.InvocationType = InvocationType.RequestResponse;
            request.FunctionName = GetDocumentFunction;
            request.Payload = JsonSerializer.Serialize(doc);
            var response = await _lambda.InvokeAsync(request, ct);
            return await JsonSerializer.DeserializeAsync<Document>(response.Payload, _jsonDeserializeOptions, cancellationToken: ct);
        }

        public override Task ListenOnChange(ListenOnChange listen, CancellationToken ct = default)
        {
            if (!string.IsNullOrEmpty(ListenOnChangeFunction))
            {
                return listenOnChangeLambda(listen, ct);
            }
            return listenOnChangeSQS(listen, ct);
        }
        public async Task listenOnChangeSQS(ListenOnChange listen, CancellationToken ct)
        {
            var request = new SendMessageRequest();
            request.QueueUrl = ListenOnChangeQueueUrl;
            request.MessageBody = JsonSerializer.Serialize(listen);
            await _sqs.SendMessageAsync(request, ct);
        }

        public async Task listenOnChangeLambda(ListenOnChange listen, CancellationToken ct)
        {
            var request = new InvokeRequest();
            request.InvocationType = InvocationType.Event;
            request.FunctionName = ListenOnChangeFunction;
            request.Payload = JsonSerializer.Serialize(listen);
            await _lambda.InvokeAsync(request, ct);
        }
    }
}