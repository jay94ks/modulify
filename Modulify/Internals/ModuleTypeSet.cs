using System;
using System.Collections.Generic;

namespace Modulify.Internals
{
    /// <summary>
    /// Default implementation of <see cref="IModuleTypeSet"/>.
    /// </summary>
    public class ModuleTypeSet : HashSet<Type>, IModuleTypeSet
    {
        /// <summary>
        /// Initialize a new <see cref="ModuleTypeSet"/> instance.
        /// </summary>
        public ModuleTypeSet() { }

        /// <summary>
        /// Initialize a new <see cref="ModuleTypeSet"/> instance.
        /// </summary>
        /// <param name="Types"></param>
        public ModuleTypeSet(IModuleTypeSet Types) : base(Types) { }
    }
}
