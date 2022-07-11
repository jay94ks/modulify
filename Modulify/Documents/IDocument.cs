using System;

namespace Modulify.Documents
{
    /// <summary>
    /// Doucment interface.
    /// </summary>
    public interface IDocument
    {
        /// <summary>
        /// Guid that is used to identify the document.
        /// </summary>
        Guid Guid { get; }
    }
}
