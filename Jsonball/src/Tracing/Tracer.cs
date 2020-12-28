using System.Text;
using System.Collections.Generic;

namespace TFaller.Jsonball.Client.Tracing
{
    /// <summary>
    /// A Tracer holds a hierarchical list of visited property values.
    /// </summary>
    public class Tracer : Dictionary<string, Tracer>
    {
        public Tracer Trace(string name)
        {
            Tracer t;

            if (!this.TryGetValue(name, out t))
            {
                this.Add(name, t = new Tracer());
            }

            return t;
        }

        /// <summary>
        /// Converts the tracer into a array of json pointers.
        /// </summary>
        /// <returns>The pointers</returns>
        public string[] ToPointer() => ToPointer(string.Empty);

        /// <summary>
        /// Converts the tracer into a array of json pointers.
        /// </summary>
        /// <param name="path">root path</param>
        /// <returns>The pointers</returns>
        public string[] ToPointer(string path)
        {
            var pointers = new List<string>();
            ToPointer(path, pointers);
            return pointers.ToArray();
        }

        /// <summary>
        /// Converts the tracer into a list of json pointers.
        /// </summary>
        /// <param name="path">The current path</param>
        /// <param name="pointers">The list where the pointer are added</param>
        public void ToPointer(string path, List<string> pointers)
        {
            foreach (var subT in this)
            {
                var newPath = path + "/" + espacePointerPart(subT.Key);
                pointers.Add(newPath);
                subT.Value.ToPointer(newPath, pointers);
            }
        }

        /// <summary>
        /// Escapes "~" to "~0" and "/" to "~1"
        /// </summary>
        /// <param name="p">A JSON Pointer part</param>
        /// <returns>The escaped json pointer part</returns>
        private string espacePointerPart(string p)
        {
            return new StringBuilder(p)
                .Replace("~", "~0")
                .Replace("/", "~1")
                .ToString();
        }
    }
}