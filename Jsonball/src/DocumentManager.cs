using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TFaller.Jsonball.Client.Events;

namespace TFaller.Jsonball.Client
{
    /// <summary>
    /// DocumentManager is a simple way to interact with documents.
    /// The Manager can be preloaded with documents from a given change event.
    /// Documents can be simply loaded by type and name. If the given document is
    /// currently not loaded, it will be loaded automatically from the jsonball service.
    /// </summary>
    public class DocumentManager
    {
        private readonly JsonballClient _client;

        /// <summary>
        /// Holds all loaded documents by type and name.
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, Document>> _docs = new Dictionary<string, Dictionary<string, Document>>();

        /// <summary>
        /// Cache for the DocumentTypeAttribute values.
        /// </summary>
        private readonly Dictionary<Type, string> _type2docType = new Dictionary<Type, string>();

        public DocumentManager(JsonballClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Initializes the document manager, with preloaded
        /// documents of a change event.
        /// </summary>
        /// <param name="client">Client to load documents</param>
        /// <param name="change">Change Event to preload the manager</param>
        public DocumentManager(JsonballClient client, Change change)
        {
            _client = client;

            // parse change
            foreach (var doc in change.Documents)
            {
                addDocument(doc);
            }
        }

        /// <summary>
        /// Gets all currently loaded documents of a given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The jsonball document type name</param>
        /// <returns>All loaded documents of the given type</returns>
        public IDictionary<string, T> GetDocuments<T>(string type)
        {
            var dic = new Dictionary<string, T>();
            lock (_docs)
            {
                Dictionary<string, Document> docs;
                if (_docs.TryGetValue(type, out docs))
                {
                    foreach (var doc in docs)
                    {
                        dic.Add(doc.Key, (T)doc.Value.Body);
                    }
                }
            }
            return dic;
        }

        /// <summary>
        /// Gets a specific document by name. The function may return instantly if the
        /// document is already loaded. Otherwise the document is loaded from the jsonball service.
        /// </summary>
        /// <typeparam name="T">The type of the document (must have DocumentType Attribute)</typeparam>
        /// <param name="name">The document name</param>
        /// <param name="ct">Optional cancallation token</param>
        /// <returns>The document</returns>
        public ValueTask<T> GetDocumentAsync<T>(string name, CancellationToken ct = default)
        {
            string docType;
            var type = typeof(T);
            lock (_type2docType)
            {
                if (!_type2docType.TryGetValue(type, out docType))
                {
                    var attr = type.GetCustomAttributes(typeof(DocumentTypeAttribute), false);
                    if (attr.Length != 1)
                    {
                        throw new ArgumentException("Type does not have DocumentType attribute");
                    }
                    _type2docType.Add(type, docType = ((DocumentTypeAttribute)attr[0]).Type);
                }
            }
            return GetDocumentAsync<T>(docType, name, ct);
        }

        /// <summary>
        /// Gets a specific document by type and name. The function may return instantly if the
        /// document is already loaded. Otherwise the document is loaded from the jsonball service.
        /// </summary>
        /// <typeparam name="T">The previous deserialization target type</typeparam>
        /// <param name="type">The document jsonball type name</param>
        /// <param name="name">The document name</param>
        /// <param name="ct">Optional cancallation token</param>
        /// <returns>The document</returns>
        public ValueTask<T> GetDocumentAsync<T>(string type, string name, CancellationToken ct = default)
        {
            Document doc = getDocument(type, name);
            if (doc != null)
            {
                // we have the document already loaded
                return new ValueTask<T>(((T)doc.Body));
            }

            // we don't have the document cached ... we have to get it first
            return new ValueTask<T>(getDocumentAsync<T>(type, name, ct));
        }

        private async Task<T> getDocumentAsync<T>(string type, string name, CancellationToken ct)
        {
            var doc = await _client.GetDocumentAsync(new GetDocument() { Type = type, Name = name }, ct);

            if (doc.Type != type)
            {
                throw new Exception(String.Format("Expected document type {0} but got {1}", type, doc.Type));
            }
            if (doc.Name != name)
            {
                throw new Exception(String.Format("Expected document {0} but got {1}", name, doc.Name));
            }

            addDocument(doc);
            return (T)doc.Body;
        }

        private void addDocument(Document doc)
        {
            var docType = doc.Type;
            lock (_docs)
            {
                Dictionary<string, Document> sameTypeDocs;
                if (!_docs.TryGetValue(docType, out sameTypeDocs))
                {
                    _docs.Add(docType, sameTypeDocs = new Dictionary<string, Document>());
                }
                sameTypeDocs.Add(doc.Name, doc);
            }
        }

        private Document getDocument(string type, string name)
        {
            lock (_docs)
            {
                Dictionary<string, Document> sameTypeDocs;
                if (!_docs.TryGetValue(type, out sameTypeDocs))
                {
                    return null;
                }

                Document doc;
                sameTypeDocs.TryGetValue(name, out doc);
                return doc;
            }
        }
    }
}