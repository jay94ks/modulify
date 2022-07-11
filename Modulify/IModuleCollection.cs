using System;
using System.Collections.Generic;

namespace Modulify
{
    /// <summary>
    /// Module collection.
    /// </summary>
    public interface IModuleCollection : ICollection<IModule>
    {
        /// <summary>
        /// Base Type collection that are used for classification.
        /// </summary>
        IModuleTypeSet BaseTypes { get; }
        
        /// <summary>
        /// Add an <see cref="IModule"/> instance to the collection.
        /// </summary>
        /// <param name="Module"></param>
        /// <returns></returns>
        new IModuleCollection Add(IModule Module);

        /// <summary>
        /// Find an <see cref="IModule"/> instance.
        /// </summary>
        /// <param name="Predicate"></param>
        /// <returns></returns>
        IModule Find(Func<IModule, bool> Predicate);

        /// <summary>
        /// Find all <see cref="IModule"/> instances.
        /// </summary>
        /// <param name="Predicate"></param>
        /// <returns></returns>
        IEnumerable<IModule> FindAll(Func<IModule, bool> Predicate);

        /// <summary>
        /// Build the module provider instance.
        /// </summary>
        /// <returns></returns>
        IModuleProvider Build();
    }
}
