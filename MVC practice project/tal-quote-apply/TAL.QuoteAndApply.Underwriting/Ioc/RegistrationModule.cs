using TAL.QuoteAndApply.Infrastructure.Ioc;
using TAL.QuoteAndApply.Underwriting.Configuration;
using TAL.QuoteAndApply.Underwriting.Models.Converters;
using TAL.QuoteAndApply.Underwriting.Service;

namespace TAL.QuoteAndApply.Underwriting.Ioc
{
    public class RegistrationModule : IocModuleRegistration
    {
        public override void Register(ISimpleDependencyMapper mapper)
        {
            mapper
                .Add<CreateInterviewRequestProvider>()
                .Add<UpdateUnderwritingInterview>()
                .Add<CreateUnderwritingInterview>()
                .Add<UnderwritingWebServiceUrlProvider>()
                .Add<UnderwritingConfigurationProvider>()
                .Add<UnderwritingTemplateService>()
                .Add<GetUnderwritingInterview>()
                .Add<UnderwritingBenefitResponsesChangeParamConverter>()
                .Add<UnderwritingTagMetaDataService>()
                .Add<UnderwritingQuestionAnswerProvider>()
                .Add<TalusUiTokenService>()
                .Add<CompleteUnderwritingInterview>();
        }
    }
}
