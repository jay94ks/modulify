using System;
using System.Collections.Generic;

namespace Modulify.DependencyInjection.Extensions
{
    public static partial class ModulifyExtensions
    {
        private class HiddenModuleRegistration
        {
            /// <summary>
            /// Singleton factories.
            /// </summary>
            public List<Func<IServiceProvider, IModule>> Singleton = new List<Func<IServiceProvider, IModule>>();

            /// <summary>
            /// Scoped factories.
            /// </summary>
            public List<Func<IServiceProvider, IModule>> Scoped = new List<Func<IServiceProvider, IModule>>();
        }

        
    }
}
