using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulify.Internals
{
    /// <summary>
    /// Default implementation of <see cref="ModuleProvider"/>.
    /// </summary>
    public class ModuleProvider : IModuleProvider
    {
        private Dictionary<Type, IModule[]> m_Modules;
        private HashSet<IModule> m_Elses;

        /// <summary>
        /// Initialize a new <see cref="ModuleProvider"/> instance.
        /// </summary>
        /// <param name="Collection"></param>
        public ModuleProvider(IModuleCollection Collection)
        {
            m_Modules = new Dictionary<Type, IModule[]>();
            m_Elses = new HashSet<IModule>(Collection.Where(X => !Collection.BaseTypes.CanCover(X.GetType())));
            OrganizeModuleDictionary(Collection);
        }

        /// <summary>
        /// Organize the module dictionary using the collection.
        /// </summary>
        /// <param name="Collection"></param>
        private void OrganizeModuleDictionary(IModuleCollection Collection)
        {
            var Temp = new Dictionary<Type, List<IModule>>();
            foreach (var Each in Collection.Where(X => !m_Elses.Contains(X)))
            {
                var Covers = Collection.BaseTypes.FindCovers(Each.GetType());
                foreach (var EachType in Covers)
                    Temp.GetOrNew(EachType).Add(Each);
            }

            foreach (var Each in Temp)
                m_Modules[Each.Key] = Each.Value.ToArray();
        }

        /// <inheritdoc/>
        public IModule Find(Type BaseType) => FindAll(BaseType).LastOrDefault();

        /// <inheritdoc/>
        public IModule Find(Type BaseType, Func<IModule, bool> Predicate) 
            => FindAll(BaseType, Predicate).LastOrDefault();

        /// <inheritdoc/>
        public IEnumerable<IModule> FindAll(Type BaseType)
        {
            if (m_Modules.TryGetValue(BaseType, out var Modules))
                return Modules;

            return m_Elses.Where(X => BaseType.IsAssignableFrom(X.GetType()));
        }

        /// <inheritdoc/>
        public IEnumerable<IModule> FindAll(Type BaseType, Func<IModule, bool> Predicate)
        {
            if (m_Modules.TryGetValue(BaseType, out var Modules))
                return Modules.Where(Predicate);

            return m_Elses.Where(X
                => BaseType.IsAssignableFrom(X.GetType()) && Predicate(X));
        }
    }
}
