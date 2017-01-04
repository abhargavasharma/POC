using System.Collections.Generic;

namespace TAL.QuoteAndApply.Infrastructure.Ioc
{
    public abstract class IocModuleRegistration
    {
        public abstract void Register(ISimpleDependencyMapper mapper);

        public List<TypeMapItem> GetMappings()
        {
            using (var dependencyMapper = new SimpleDependencyMapper())
            {
                Register(dependencyMapper);
                return dependencyMapper.TypeMaps;
            }
        }
    }
}
