using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TAL.QuoteAndApply.Infrastructure.Ioc
{
    public class IocRegistrationResolver
    {
        private readonly IDictionary<string, Assembly> _assemblies;

        public IocRegistrationResolver()
        {
            _assemblies = new Dictionary<string, Assembly>();
        }

        public void LoadAssembly(Assembly assembly, params string[] assemblyStartsWith)
        {
            if (_assemblies.ContainsKey(assembly.FullName))
                return;

            _assemblies.Add(assembly.FullName, assembly);

            var references = assembly.GetReferencedAssemblies();
            foreach (var assemblyName in GetReferencedAssemblies(references, assemblyStartsWith))
            {
                if (_assemblies.ContainsKey(assemblyName.FullName))
                    continue;

                var referencedAssembly = Assembly.Load(assemblyName);

                var children = GetEnumerableOfType<IocModuleRegistration>(new[] { referencedAssembly });
                if (children.Any())
                    LoadAssembly(referencedAssembly);
            }
        }

        public List<TypeMapItem> GetMappings()
        {
            var lastSeen = new List<TypeMapItem>();
            var modules = GetEnumerableOfType<IocModuleRegistration>(_assemblies.Values);
            foreach (var iocModuleRegistration in modules)
            {
                var mappings = iocModuleRegistration.GetMappings();
                lastSeen = lastSeen.Concat(mappings).ToList();
            }

            return lastSeen;
        }

        private IEnumerable<AssemblyName> GetReferencedAssemblies(IEnumerable<AssemblyName> assemblyNames, string[] assemblyStartsWith)
        {
            return assemblyNames.Where(r => !assemblyStartsWith.Any() ||
                assemblyStartsWith.Any(assemblyName => r.FullName.StartsWith(assemblyName, StringComparison.OrdinalIgnoreCase)));
        }

        private static IEnumerable<T> GetEnumerableOfType<T>(IEnumerable<Assembly> assemblies) where T : class
        {
            // throw exception as no parameter less contructor found
            var objects = assemblies.SelectMany(a => a.GetTypes())
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T)) &&
                                 myType.GetConstructors().FirstOrDefault(c => c.GetParameters().Length == 0) != null)
                .Select(type => (T)Activator.CreateInstance(type))
                .ToList();
            return objects;
        }
    }
}
