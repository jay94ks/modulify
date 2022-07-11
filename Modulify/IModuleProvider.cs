using System;
using System.Collections.Generic;

namespace Modulify
{
    /// <summary>
    /// Module Provider interface.
    /// </summary>
    public interface IModuleProvider
    {
        /// <summary>
        /// Find an <see cref="IModule"/> instance that based on the given type.
        /// </summary>
        /// <param name="BaseType"></param>
        /// <returns></returns>
        IModule Find(Type BaseType);

        /// <summary>
        /// Find an <see cref="IModule"/> instance that based on the given type
        /// and represent at first of the registered modules.
        /// </summary>
        /// <param name="BaseType"></param>
        /// <param name="Predicate"></param>
        /// <returns></returns>
        IModule Find(Type BaseType, Func<IModule, bool> Predicate);

        /// <summary>
        /// Find module instances.
        /// </summary>
        /// <param name="BaseType"></param>
        /// <returns></returns>
        IEnumerable<IModule> FindAll(Type BaseType);

        /// <summary>
        /// Find module instances that mets the predicate.
        /// </summary>
        /// <param name="BaseType"></param>
        /// <param name="Predicate"></param>
        /// <returns></returns>
        IEnumerable<IModule> FindAll(Type BaseType, Func<IModule, bool> Predicate);
    }
}
