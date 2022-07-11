using Newtonsoft.Json.Linq;

namespace Modulify.Documents
{
    /// <summary>
    /// Provides the document serialization and de-serialization.
    /// </summary>
    public interface IDocumentTextModule : IDocumentModule
    {
        /// <summary>
        /// Test whether the input can be deserialized by this module or not.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        bool CanSupport(JObject Input);

        /// <summary>
        /// Serialize the document into the <see cref="JObject"/>.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        JObject Serialize(IDocument Input);

        /// <summary>
        /// Deserialize the document from the <see cref="JObject"/>.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        IDocument Deserialize(JObject Input);
    }
}
