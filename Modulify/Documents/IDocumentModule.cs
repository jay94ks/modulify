using System;

namespace Modulify.Documents
{
    /// <summary>
    /// Describes the <see cref="IDocument"/> module.
    /// </summary>
    public interface IDocumentModule : IModule
    {
        /// <summary>
        /// Test whether the input doucment can be serialized by this module or not.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        bool CanSupport(IDocument Input);

        /// <summary>
        /// Create a new <see cref="IDocument"/> instance.
        /// </summary>
        /// <returns></returns>
        IDocument CreateNew();
    }
}
