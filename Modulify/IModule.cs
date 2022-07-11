using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulify
{
    /// <summary>
    /// Module interface that describes itself.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Name of the module that is human readable.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Alias name that used to express the module itself shortly.
        /// e.g. editor, listener, blah blah....
        /// </summary>
        string Alias { get; }
    }
}
