using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulify.Documents.Internals
{
    internal class ForcedBinarySupport : IDocumentBinaryModule
    {
        private IDocumentTextModule m_Module;

        /// <summary>
        /// Initialize a new <see cref="ForcedBinarySupport"/> instance.
        /// </summary>
        /// <param name="Module"></param>
        public ForcedBinarySupport(IDocumentTextModule Module) => m_Module = Module;

        /// <inheritdoc/>
        public string Name => m_Module.Name;

        /// <inheritdoc/>
        public string Alias => m_Module.Alias;

        /// <inheritdoc/>
        public bool CanSupport(Stream Input)
        {
            using (var Reader = new StreamReader(Input, Encoding.UTF8, true, -1, true))
            {
                try
                {
                    var Json = JsonConvert.DeserializeObject<JObject>(Reader.ReadToEnd());
                    return Json != null && m_Module.CanSupport(Json);
                }

                catch { }
            }

            return false;
        }

        /// <inheritdoc/>
        public bool CanSupport(IDocument Input) => m_Module.CanSupport(Input);

        /// <inheritdoc/>
        public IDocument CreateNew() => m_Module.CreateNew();

        /// <inheritdoc/>
        public IDocument Deserialize(Stream Input)
        {
            using (var Reader = new StreamReader(Input, Encoding.UTF8, true, -1, true))
            {
                var Json = JsonConvert.DeserializeObject<JObject>(Reader.ReadToEnd());
                if (Json != null)
                    return m_Module.Deserialize(Json);

            }
            return null;
        }

        /// <inheritdoc/>
        public void Serialize(Stream Output, IDocument Input)
        {
            var Json = m_Module.Serialize(Input) ?? new JObject();
            var Bytes = Encoding.UTF8.GetBytes(Json.ToString());
            Output.Write(Bytes, 0, Bytes.Length);
        }
    }
}
