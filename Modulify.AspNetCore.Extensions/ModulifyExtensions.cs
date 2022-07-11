using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modulify.Internals;
using System;
using System.Linq;

namespace Modulify.DependencyInjection.Extensions
{
    public static partial class ModulifyExtensions
    {
        /// <summary>
        /// Null Module Provider that if no module provider registered for the service collection.
        /// </summary>
        private static readonly ModuleProvider Null = new ModuleProvider(new ModuleCollection());

        /// <summary>
        /// Configures the <see cref="IModuleCollection"/> for the modulify framework.
        /// </summary>
        /// <param name="Services"></param>
        /// <param name="Configure"></param>
        /// <returns></returns>
        public static IServiceCollection Modulify(this IServiceCollection Services, Action<IModuleCollection> Configure = null)
        {
            var Descriptor = Services.FirstOrDefault(X => X.ServiceType == typeof(HiddenModuleCollection));
            if (Descriptor is null || Descriptor.ImplementationInstance is not HiddenModuleCollection Collection)
            {
                Services
                    .AddSingleton(Collection = new HiddenModuleCollection())

                    // --> register HiddenModuleRegistration for covers the singleton module provider.
                    .AddSingleton(Services =>
                    {
                        var Registrations = Services.GetService<HiddenModuleRegistration>();
                        if (Registrations != null)
                        {
                            foreach (var Each in Registrations.Singleton)
                                Collection.Add(Each.Invoke(Services));
                        }

                        return new HiddenModuleProvider(Collection.Build());
                    })

                    // --> register the IModuleProvider instance.
                    //   : uses HiddenModuleRegistration if configured.
                    .AddScoped(Services =>
                    {
                        // --> use the factory if configured.
                        var Registrations = Services.GetService<HiddenModuleRegistration>();
                        if (Registrations != null)
                        {
                            var Scoped = new ModuleCollection(Collection);

                            foreach (var Each in Registrations.Scoped)
                                Scoped.Add(Each.Invoke(Services));

                            return Scoped.Build();
                        }

                        // --> or not, use the hidden instance.
                        return Services.GetService<HiddenModuleProvider>();
                    });
            }

            Configure?.Invoke(Collection);
            return Services;
        }

        /// <summary>
        /// Get the <see cref="IModuleProvider"/> from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="Services"></param>
        /// <returns></returns>
        public static IModuleProvider GetModuleProvider(this IServiceProvider Services)
        {
            return Services.GetService<IModuleProvider>() ?? Null;
        }
        
        /// <summary>
        /// Add the module to the service collection for the modulify framework.
        /// </summary>
        /// <param name="Services"></param>
        /// <param name="Factory"></param>
        /// <returns></returns>
        public static IServiceCollection AddModule(this IServiceCollection Services, IModule Module)
        {
            Services.Modulify(X => X.Add(Module));
            return Services;
        }
        
        /// <summary>
        /// Add the module factory to the service collection for the modulify framework.
        /// </summary>
        /// <param name="Services"></param>
        /// <param name="Factory"></param>
        /// <returns></returns>
        public static IServiceCollection AddModule(this IServiceCollection Services, Func<IServiceProvider, IModule> Factory)
        {
            Services.Modulify();

            var Descriptor = Services.FirstOrDefault(X => X.ServiceType == typeof(HiddenModuleRegistration));
            if (Descriptor is null || Descriptor.ImplementationInstance is not HiddenModuleRegistration Registration)
                Services.AddSingleton(Registration = new HiddenModuleRegistration());

            if (!Registration.Singleton.Contains(Factory))
                 Registration.Singleton.Add(Factory);

            return Services;
        }
        
        /// <summary>
        /// Add the module factory to the service collection for the modulify framework.
        /// </summary>
        /// <param name="Services"></param>
        /// <param name="Factory"></param>
        /// <returns></returns>
        public static IServiceCollection AddModule<TModule>(this IServiceCollection Services) where TModule: class, IModule
        {
            return Services
                .AddSingleton<TModule>()
                .AddModule(X => X.GetRequiredService<TModule>());
        }

        /// <summary>
        /// Add the module factory to the service collection for the modulify framework.
        /// </summary>
        /// <param name="Services"></param>
        /// <param name="Factory"></param>
        /// <returns></returns>
        public static IServiceCollection AddScopedModule(this IServiceCollection Services, Func<IServiceProvider, IModule> Factory)
        {
            Services.Modulify();

            var Descriptor = Services.FirstOrDefault(X => X.ServiceType == typeof(HiddenModuleRegistration));
            if (Descriptor is null || Descriptor.ImplementationInstance is not HiddenModuleRegistration Registration)
                Services.AddSingleton(Registration = new HiddenModuleRegistration());

            if (!Registration.Scoped.Contains(Factory))
                 Registration.Scoped.Add(Factory);

            return Services;
        }

        /// <summary>
        /// Add the module factory to the service collection for the modulify framework.
        /// </summary>
        /// <param name="Services"></param>
        /// <param name="Factory"></param>
        /// <returns></returns>
        public static IServiceCollection AddScopedModule<TModule>(this IServiceCollection Services) where TModule : class, IModule
        {
            return Services
                .AddScoped<TModule>()
                .AddScopedModule(X => X.GetRequiredService<TModule>());
        }

    }
}
