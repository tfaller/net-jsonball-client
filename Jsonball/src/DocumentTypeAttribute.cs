using System;

namespace TFaller.Jsonball.Client
{

    /// <summary>
    /// DocumentTypeAttribute is used to set which jsonball document type
    /// is mapped to this class. Note: This is only currently used for the
    /// Document Manager.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class DocumentTypeAttribute : Attribute
    {
        private string _type;

        public DocumentTypeAttribute(string type)
        {
            _type = type;
        }

        /// <summary>
        /// The jsonball document type name.
        /// </summary>
        public string Type { get => _type; }
    }
}