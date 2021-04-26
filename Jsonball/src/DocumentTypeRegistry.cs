using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using TFaller.Jsonball.Client.Events;

namespace TFaller.Jsonball.Client
{
    /// <summary>
    /// The DocumentTypeRegistry is a singleton that handles deserialization of jsonball
    /// documents. The Application should register once all known document types and their
    /// correspnding deserialization type.
    /// </summary>
    public static class DocumentTypeRegistry
    {
        private static Dictionary<string, Type> _docTypes = new Dictionary<string, Type>();

        /// <summary>
        /// Registers a type for a given jsonball document type.
        /// </summary>
        /// <typeparam name="T">The type which is the deserialization target</typeparam>
        /// <param name="docTypeName">The jsonball document type name</param>
        public static void RegisterDocumentType<T>(string docTypeName)
        {
            _docTypes.Add(docTypeName, typeof(T));
        }

        internal class DocumentConverter : JsonConverter<Document>
        {

            public override Document Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                // document is always an object
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException("Expected an object, but got " + reader.TokenType);
                }

                var doc = new Document();

                // read each property
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        return doc;
                    }
                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException();
                    }

                    var propName = reader.GetString();

                    // move to property value
                    if (!reader.Read())
                    {
                        throw new JsonException();
                    }

                    switch (propName)
                    {
                        case "name":
                            doc.Name = reader.GetString();
                            break;
                        case "type":
                            doc.Type = reader.GetString();
                            break;
                        case "version":
                            doc.Version = reader.GetUInt32();
                            break;
                        case "doc":
                            Type docType;
                            if (!DocumentTypeRegistry._docTypes.TryGetValue(doc.Type, out docType))
                            {
                                throw new JsonException("Unknown document type: " + doc.Type);
                            }
                            doc.Body = JsonSerializer.Deserialize(ref reader, docType, options);
                            break;
                        default:
                            throw new JsonException("Invalid document property: " + propName);
                    }
                }

                throw new JsonException();
            }

            public override void Write(Utf8JsonWriter writer, Document value, JsonSerializerOptions options)
            {
                // for now we don't have to serialize this class
                throw new NotImplementedException();
            }
        }
    }
}