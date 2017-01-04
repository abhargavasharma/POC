using TAL.QuoteAndApply.Infrastructure.Ioc;
using TAL.QuoteAndApply.Search.Configuration;
using TAL.QuoteAndApply.Search.Data;
using TAL.QuoteAndApply.Search.Service;

namespace TAL.QuoteAndApply.Search.Ioc
{
    public class RegistrationModule : IocModuleRegistration
    {
        public override void Register(ISimpleDependencyMapper mapper)
        {
            mapper.WhenRequesting<IQuestionAnswerReaderFactory>().ProvideImplementationOf<QuestionAnswerReaderFactory>()
                .WhenRequesting<IQuestionAnswerWriterFactory>().ProvideImplementationOf<QuestionAnswerWriterFactory>()
                .WhenRequesting<ISearchConfigurationProvider>().ProvideImplementationOf<SearchConfigurationProvider>()
                .WhenRequesting<IAnswerSearchItemRepository>().ProvideImplementationOf<AnswerSearchItemRepository>();
        }
    }
}
