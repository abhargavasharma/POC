using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TAL.QuoteAndApply.Infrastructure.Ioc
{
    public static class SubClassActivator
    {
        private static readonly IDictionary<string, Assembly> _assemblies;

        static SubClassActivator()
        {
            _assemblies = new Dictionary<string, Assembly>();
        }

        public static void ActivateForAssembly<T>(Assembly assembly, params string[] assemblyStartsWith) where T : class
        {
            LoadAssembly<T>(assembly, assemblyStartsWith).ToList();
        }

        private static IEnumerable<T> LoadAssembly<T>(Assembly assembly, params string[] assemblyStartsWith) where T : class
        {
            if (!_assemblies.ContainsKey(assembly.FullName))
            {
                _assemblies.Add(assembly.FullName, assembly);

                var instances = GetEnumerableOfType<T>(new[] { assembly });
                foreach (var instance in instances)
                {
                    yield return instance;
                }

                var references = assembly.GetReferencedAssemblies();
                foreach (var assemblyName in GetReferencedAssemblies(references, assemblyStartsWith))
                {
                    var referencedAssembly = Assembly.Load(assemblyName);
                    var intancesOfReference = LoadAssembly<T>(referencedAssembly, assemblyStartsWith);
                    
                    foreach (var instance in intancesOfReference)
                    {
                        yield return instance;
                    }
                }
            }
        }

        private static IEnumerable<AssemblyName> GetReferencedAssemblies(IEnumerable<AssemblyName> assemblyNames, string[] assemblyStartsWith)
        {
            return assemblyNames.Where(r => !assemblyStartsWith.Any() ||
                                            assemblyStartsWith.Any(assemblyName => r.FullName.StartsWith(assemblyName, StringComparison.OrdinalIgnoreCase)));
        }

        private static IEnumerable<T> GetEnumerableOfType<T>(IEnumerable<Assembly> assemblies) where T : class
        {
            // throw exception as no parameter less contructor found
            var objects = assemblies.SelectMany(a => a.GetTypes())
                .Where(myType => myType.IsClass && !myType.IsAbstract && typeof(T).IsAssignableFrom(myType) &&
                                 myType.GetConstructors().FirstOrDefault(c => c.GetParameters().Length == 0) != null)
                .Select(type => (T)Activator.CreateInstance(type))
                .ToList();
            return objects;
        }

    }
}