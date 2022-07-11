using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modulify
{
    public static class ModuleExtensions
    {
        /// <summary>
        /// Test whether the <paramref name="ChildType"/> can be covered by the type set or not.
        /// </summary>
        /// <param name="TypeSet"></param>
        /// <param name="ChildType"></param>
        /// <returns></returns>
        internal static bool CanCover(this IModuleTypeSet TypeSet, Type ChildType)
        {
            if (TypeSet is null || ChildType is null)
                return false;

            return TypeSet.FirstOrDefault(X => X.IsAssignableFrom(ChildType)) != null;
        }

        /// <summary>
        /// Find all coverable types for the child type.
        /// </summary>
        /// <param name="TypeSet"></param>
        /// <param name="ChildType"></param>
        /// <returns></returns>
        internal static IEnumerable<Type> FindCovers(this IModuleTypeSet TypeSet, Type ChildType)
        {
            if (TypeSet is null)
                yield break;

            foreach(var Each in TypeSet)
            {
                if (ChildType.IsAssignableTo(Each))
                    yield return Each;
            }
        }

        /// <summary>
        /// Get or new an item for the map.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="Map"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        internal static TValue GetOrNew<TValue, TKey>(this IDictionary<TKey, TValue> Map, TKey Key) where TValue : new()
        {
            if (Map.TryGetValue(Key, out TValue Return))
                return Return;

            return Map[Key] = new TValue();
        }

        /// <summary>
        /// Cast the module instance as <typeparamref name="TModule"/> instance.
        /// </summary>
        /// <typeparam name="TModule"></typeparam>
        /// <param name="Module"></param>
        /// <returns></returns>
        public static TModule As<TModule>(this IModule Module) where TModule : class, IModule => Module as TModule;
    }
}
