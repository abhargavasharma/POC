using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.Infrastructure.Ioc;

namespace TAL.QuoteAndApply.DataLayer.Ioc
{
    public class RegistrationModule : IocModuleRegistration
    {
        public override void Register(ISimpleDependencyMapper mapper)
        {
            mapper.Add<DataLayerExceptionFactory>();
            mapper.Add<SimpleEncryptionService>();
            mapper.Add<DbItemEncryptionService>();
        }
    }
}
