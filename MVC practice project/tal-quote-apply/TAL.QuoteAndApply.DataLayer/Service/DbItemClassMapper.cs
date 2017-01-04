using System.Reflection;
using DapperExtensions;
using TAL.QuoteAndApply.DataLayer.Service.Dapper;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.Infrastructure.Ioc;

namespace TAL.QuoteAndApply.DataLayer.Service
{
    public abstract class DbItemClassMapper<T> : IDbItemClassMapper where T : DbItem
    {
        public static void RegisterClassMaps()
        {
            SubClassActivator.ActivateForAssembly<IDbItemClassMapper>(Assembly.GetCallingAssembly(), "TAL.QuoteAndApply");
        }

        public static void RegisterClassMaps(Assembly callingAssembly)
        {
            SubClassActivator.ActivateForAssembly<IDbItemClassMapper>(callingAssembly, "TAL.QuoteAndApply");
        }

        protected DbItemClassMapper()
        {
            var dapperExtMapper = new CustomClassMapper<T>();
            DapperExtentionMappers.Instance.RegisterClassMap(dapperExtMapper);

            dapperExtMapper.Map(x => x.RV).RowVersion();
            dapperExtMapper.Map(x => x.IsEncrypted).Ignore();

            DefineMappings(new ClassMapperWrapper<T>(dapperExtMapper));
            dapperExtMapper.AutoMap();
        }

        protected abstract void DefineMappings(ClassMapperWrapper<T> mapper);
    }
}