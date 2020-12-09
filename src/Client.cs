using System.Threading;
using System.Threading.Tasks;
using TFaller.Jsonball.Client.Events;

namespace TFaller.Jsonball.Client
{

    public abstract class Client
    {
        protected string CreateDocName(string type, string name)
        {
            return string.Format("{0}:{1}:{2}", type.Length, type, name);
        }

        public abstract Task PostDocumentAsync(PostDocument doc, CancellationToken ct = default);

        public abstract Task<Document> GetDocumentAsync(GetDocument doc, CancellationToken ct = default);

        public abstract Task ListenOnChange(ListenOnChange listen, CancellationToken ct = default);
    }

}