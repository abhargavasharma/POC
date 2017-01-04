using TAL.QuoteAndApply.Infrastructure.Ioc;
using TAL.QuoteAndApply.PremiumCalculation.Configuration;
using TAL.QuoteAndApply.PremiumCalculation.Data;
using TAL.QuoteAndApply.PremiumCalculation.Services;

namespace TAL.QuoteAndApply.PremiumCalculation.Ioc
{
    public class RegistrationModule : IocModuleRegistration
    {
        public override void Register(ISimpleDependencyMapper mapper)
        {
            mapper
               .Add<PremiumCalculationService>();

            RegisterRepositories(mapper);
            RegisterServices(mapper);
            RegisterConfiguration(mapper);
        }

        private void RegisterConfiguration(ISimpleDependencyMapper mapper)
        {
            mapper
                .Add<PremiumCalculationConfigurationProvider>();
        }

        private void RegisterServices(ISimpleDependencyMapper mapper)
        {
            mapper
               .Add<PremiumCalculationConfigurationProvider>()
               .Add<CoverBaseRateLookupRequestProvider>()
               .Add<GetCoverCalculatorInputService>()
               .Add<GetFactorACalculatorInputService>()
               .Add<GetFactorBCalculatorInputService>()
               .Add<GetMultiCoverDiscountCalculatorInputService>()
               .Add<GetRiskCalculatorInputService>()
               .Add<GetPlanCalculatorInputService>()
               .Add<GetMultiCoverBlockCalculatorInputService>()
               .Add<GetLoadingCalculatorInputService>()
               .Add<GetDayOneAccidentCalculatorInputService>()
               .Add<MultiPlanDiscountFactorService>();
        }

        private void RegisterRepositories(ISimpleDependencyMapper mapper)
        {
            mapper
                .Add<CoverBaseRateDtoRepository>()
                .Add<LargeSumInsuredDiscountFactorDtoRepository>()
                .Add<OccupationClassFactorDtoRepository>()
                .Add<OccupationMappingDtoRepository>()
                .Add<SmokerFactorDtoRepository>()
                .Add<WaitingPeriodFactorDtoRepository>()
                .Add<CoverDivisionalFactorDtoRepository>()
                .Add<MultiPlanDiscountFactorDtoRepository>()
                .Add<ModalFrequencyFactorDtoRepository>()
                .Add<MultiCoverDiscountFactorDtoRepository>()
                .Add<PremiumReliefFactorDtoRepository>()
                .Add<PerMilleLoadingFactorDtoRepository>()
                .Add<PercentageLoadingFactorDtoRepository>()
                .Add<IncreasingClaimsFactorDtoRepository>()
                .Add<IndemnityFactorDtoRepository>()
                .Add<DayOneAccidentBaseRateDtoRepository>()
                .Add<PlanMinimumCoverAmountForMultiPlanDiscountDtoRepository>()
                .Add<OccupationDefinitionTypeFactorDtoRepository>();
        }
    }
}
