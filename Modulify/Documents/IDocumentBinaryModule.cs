using System.IO;

namespace Modulify.Documents
{
    /// <summary>
    /// Provides the document serialization and de-serialization.
    /// </summary>
    public interface IDocumentBinaryModule : IDocumentModule
    {
        /// <summary>
        /// Test whether the input can be deserialized by this module or not.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        bool CanSupport(Stream Input);

        /// <summary>
        /// Serialize the document into the <see cref="Stream"/>.
        /// </summary>
        /// <param name="Output"></param>
        /// <param name="Input"></param>
        void Serialize(Stream Output, IDocument Input);

        /// <summary>
        /// Deserialize the document from the <see cref="Stream"/>.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        IDocument Deserialize(Stream Input);
    }
}
