using System;
using System.Collections.Generic;

namespace Modulify.DependencyInjection.Extensions
{
    public static partial class ModulifyExtensions
    {
        /// <summary>
        /// Hidden Module Provider.
        /// </summary>
        private class HiddenModuleProvider : IModuleProvider
        {
            private IModuleProvider m_Modules;

            /// <summary>
            /// Initialize a new <see cref="HiddenModuleProvider"/> instance.
            /// </summary>
            /// <param name="Modules"></param>
            public HiddenModuleProvider(IModuleProvider Modules) => m_Modules = Modules;

            /// <inheritdoc/>
            public IModule Find(Type BaseType) => m_Modules.Find(BaseType);

            /// <inheritdoc/>
            public IModule Find(Type BaseType, Func<IModule, bool> Predicate) => m_Modules.Find(BaseType, Predicate);

            /// <inheritdoc/>
            public IEnumerable<IModule> FindAll(Type BaseType) => m_Modules.FindAll(BaseType);

            /// <inheritdoc/>
            public IEnumerable<IModule> FindAll(Type BaseType, Func<IModule, bool> Predicate) => m_Modules.FindAll(BaseType, Predicate);
        }
    }
}
