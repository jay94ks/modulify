using System;
using System.Collections.Generic;
using System.Linq;

namespace Modulify.Internals
{

    /// <summary>
    /// Default implementation of <see cref="IModuleCollection"/>.
    /// </summary>
    public class ModuleCollection : List<IModule>, IModuleCollection
    {
        private ModuleTypeSet m_BaseTypes;

        /// <summary>
        /// Initialize a new <see cref="ModuleCollection"/> instance.
        /// </summary>
        public ModuleCollection() => m_BaseTypes = new ModuleTypeSet();

        /// <summary>
        /// Initialize a new <see cref="ModuleCollection"/> instance.
        /// </summary>
        /// <param name="Collection"></param>
        public ModuleCollection(IModuleCollection Collection) : base(Collection)
            => m_BaseTypes = new ModuleTypeSet(Collection.BaseTypes);

        /// <inheritdoc/>
        public IModuleTypeSet BaseTypes => m_BaseTypes;

        /// <inheritdoc/>
        public new IModuleCollection Add(IModule Module)
        {
            base.Add(Module);
            return this;
        }

        /// <inheritdoc/>
        public IModule Find(Func<IModule, bool> Predicate) 
            => base.Find(X => Predicate(X));

        /// <inheritdoc/>
        public IEnumerable<IModule> FindAll(Func<IModule, bool> Predicate)
            => base.FindAll(X => Predicate(X));

        /// <inheritdoc/>
        public virtual IModuleProvider Build() => new ModuleProvider(this);
    }
}
