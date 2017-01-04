using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.PremiumCalculation.Configuration;
using TAL.QuoteAndApply.PremiumCalculation.Data;
using TAL.QuoteAndApply.PremiumCalculation.Models;
using TAL.QuoteAndApply.PremiumCalculation.Services;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.PremiumCalculation.IntegrationTests
{
    [TestFixture, Description("From the algorithm spreadsheet")]
    public class PremiumCalculationServiceTests
    {
        public const int BrandId = 1;

        private PremiumCalculationService _premiumCalculationService;

        [TestFixtureSetUp]
        public void Setup()
        {
            var premiumCalculationConfigurationProvider = new PremiumCalculationConfigurationProvider();
            var currentUserProvider = new MockCurrentUserProvider();
            var dataLayerExceptionFactory = new DataLayerExceptionFactory();
            var dbItemEncryptionService = new DbItemEncryptionService(new SimpleEncryptionService());
            var cachingService = new CachingWrapper(new MockHttpProvider());

            var coverDivisionalFactorRepo = new CoverDivisionalFactorDtoRepository(
                premiumCalculationConfigurationProvider, currentUserProvider, dataLayerExceptionFactory,
                dbItemEncryptionService, cachingService);

            var occupationMapingRepo = new OccupationMappingDtoRepository(premiumCalculationConfigurationProvider, currentUserProvider, dataLayerExceptionFactory,
                dbItemEncryptionService, cachingService);

            var coverBaseRateDtoRepository = new CoverBaseRateDtoRepository(premiumCalculationConfigurationProvider, currentUserProvider, dataLayerExceptionFactory,
               dbItemEncryptionService, cachingService);

            var multiPlanDiscountFactorDtoRepository = new MultiPlanDiscountFactorDtoRepository(premiumCalculationConfigurationProvider, currentUserProvider, dataLayerExceptionFactory,
               dbItemEncryptionService, cachingService);

            var largeSumInsuredDiscountFactorDtoRepository = new LargeSumInsuredDiscountFactorDtoRepository(premiumCalculationConfigurationProvider, currentUserProvider, dataLayerExceptionFactory,
               dbItemEncryptionService, cachingService);

            var modalFrequencyRepo = new ModalFrequencyFactorDtoRepository(premiumCalculationConfigurationProvider, currentUserProvider, dataLayerExceptionFactory,
               dbItemEncryptionService, cachingService);

            var multiCoverDiscountFactorRepo = new MultiCoverDiscountFactorDtoRepository(premiumCalculationConfigurationProvider, currentUserProvider, dataLayerExceptionFactory,
               dbItemEncryptionService, cachingService);

            var premiumReliefFactorDtoRepository = new PremiumReliefFactorDtoRepository(premiumCalculationConfigurationProvider, currentUserProvider, dataLayerExceptionFactory,
               dbItemEncryptionService, cachingService);

            var percentageLoadingFactorDtoRepository = new PercentageLoadingFactorDtoRepository(premiumCalculationConfigurationProvider, currentUserProvider, dataLayerExceptionFactory,
               dbItemEncryptionService, cachingService);

            var perMilleLoadingFactorDtoRepository = new PerMilleLoadingFactorDtoRepository(premiumCalculationConfigurationProvider, currentUserProvider, dataLayerExceptionFactory,
               dbItemEncryptionService, cachingService);

            var occupationClassFactorDtoRepository = new OccupationClassFactorDtoRepository(premiumCalculationConfigurationProvider, currentUserProvider, dataLayerExceptionFactory,
               dbItemEncryptionService, cachingService);

            var smokerFactorDtoRepository = new SmokerFactorDtoRepository(premiumCalculationConfigurationProvider, currentUserProvider, dataLayerExceptionFactory,
               dbItemEncryptionService, cachingService);

            var increasingClaimFactorDtoRepository = new IncreasingClaimsFactorDtoRepository(premiumCalculationConfigurationProvider, currentUserProvider, dataLayerExceptionFactory,
               dbItemEncryptionService, cachingService);

            var indemnityFactorDtoRepository = new IndemnityFactorDtoRepository(premiumCalculationConfigurationProvider, currentUserProvider, dataLayerExceptionFactory,
              dbItemEncryptionService, cachingService);

            var waitingPeriodFactorDtoRepository = new WaitingPeriodFactorDtoRepository(premiumCalculationConfigurationProvider, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService, cachingService);

            var dayOneAccidentBaseRateDtoRepository = new DayOneAccidentBaseRateDtoRepository(premiumCalculationConfigurationProvider, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService, cachingService);

            var planMinimumCoverAmountForMultiPlanDiscountDtoRepository = new PlanMinimumCoverAmountForMultiPlanDiscountDtoRepository(premiumCalculationConfigurationProvider, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService, cachingService);

            var occupationDefinitionTypeFactorDtoRepository = new OccupationDefinitionTypeFactorDtoRepository(premiumCalculationConfigurationProvider, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService, cachingService);

            var coverBaseRateLookupRequestProvider = new CoverBaseRateLookupRequestProvider(occupationMapingRepo);

            var multiPlanDiscountService = new MultiPlanDiscountFactorService(planMinimumCoverAmountForMultiPlanDiscountDtoRepository, multiPlanDiscountFactorDtoRepository, premiumCalculationConfigurationProvider);

            var getFactorACalculatorInputService = new GetFactorACalculatorInputService(multiPlanDiscountService, largeSumInsuredDiscountFactorDtoRepository,
                occupationClassFactorDtoRepository, smokerFactorDtoRepository, increasingClaimFactorDtoRepository, indemnityFactorDtoRepository, waitingPeriodFactorDtoRepository, occupationDefinitionTypeFactorDtoRepository);

            var getFactorBCalculatorInputService = new GetFactorBCalculatorInputService(modalFrequencyRepo, premiumReliefFactorDtoRepository);

            var getCoverCalculatorInputService = new GetCoverCalculatorInputService(coverDivisionalFactorRepo, coverBaseRateLookupRequestProvider, coverBaseRateDtoRepository, getFactorACalculatorInputService, getFactorBCalculatorInputService);

            var getMultiCoverDiscountCalculatorInputService = new GetMultiCoverDiscountCalculatorInputService(multiCoverDiscountFactorRepo);

            var riskCalculatorInputService = new GetRiskCalculatorInputService();
            var getMultiCoverBlockCalculatorInputService = new GetMultiCoverBlockCalculatorInputService(getMultiCoverDiscountCalculatorInputService);
            var getPlanCalculatorInputService = new GetPlanCalculatorInputService(getMultiCoverDiscountCalculatorInputService, multiPlanDiscountService);
            

            var getLoadingCalculatorInput = new GetLoadingCalculatorInputService(percentageLoadingFactorDtoRepository, perMilleLoadingFactorDtoRepository, getFactorBCalculatorInputService);

            var getDayOneAccidentCalculatorInputService = new GetDayOneAccidentCalculatorInputService(dayOneAccidentBaseRateDtoRepository, getFactorBCalculatorInputService, occupationClassFactorDtoRepository);

            _premiumCalculationService = new PremiumCalculationService(riskCalculatorInputService, 
                getPlanCalculatorInputService, 
                getCoverCalculatorInputService, 
                getMultiCoverBlockCalculatorInputService, 
                getLoadingCalculatorInput, 
                getDayOneAccidentCalculatorInputService);
        }

        [Test]
        public void Calculate_TestCaseOne()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var lifeAccidentCover = BuildLifeAccidentCover(0, 0);
            var lifeSportsCover = BuildLifeSportsCover(0);
            var lifePlan = BuildLifePlan(1500000, PremiumType.Stepped, true, new List<CoverCalculationRequest> { lifeAccidentCover, lifeSportsCover });

            var tpdRiderIllness = BuildTpdRiderIllnessCover(0, 0);
            var tpdRiderPlan = BuildTpdRiderPlan(1000000, PremiumType.Stepped, false, OccupationDefinition.AnyOccupation, 2, true, new List<CoverCalculationRequest> { tpdRiderIllness });

            var ciRiderAccident = BuildCiRiderAccidentCover(0, 0);
            var ciRiderIllness = BuildCiRiderIllnessCover(0, 0);
            var ciRiderCancer = BuildCiRiderCancerCover(0, 0);

            var ciRiderPlan = BuildCiRiderPlan(100000, PremiumType.Stepped, false, true,
                new List<CoverCalculationRequest> {ciRiderAccident, ciRiderIllness, ciRiderCancer});

            var risk = BuildRisk(36, Gender.Male, true, "AAA", new List<PlanCalculationRequest> { lifePlan, tpdRiderPlan, ciRiderPlan });
            var policy = new PremiumCalculationRequest(PremiumFrequency.Monthly, new List<RiskCalculationRequest> { risk }, BrandId);

            var result = _premiumCalculationService.Calculate(policy);
            var riskResult = result.RiskPremiumCalculationResults.First();

            Assert.That(riskResult.RiskPremium, Is.EqualTo(202.87m));
            Assert.That(riskResult.MultiPlanDiscount, Is.EqualTo(18.58m));

            var lifePlanResult = riskResult.PlanPremiumCalculationResults.First(p=> p.PlanCode == "DTH");
            Assert.That(lifePlanResult.TotalPremium, Is.EqualTo(79.53m));
            Assert.That(lifePlanResult.MultiPlanDiscount, Is.EqualTo(8.84m));
            Assert.That(lifePlanResult.MultiCoverDiscount, Is.EqualTo(0));

            var lifeAccCoverResult = lifePlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "DTHAC");
            Assert.That(lifeAccCoverResult.TotalPremium, Is.EqualTo(72.23m));
        
            var lifeSportsCoverResult = lifePlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "DTHASC");
            Assert.That(lifeSportsCoverResult.TotalPremium, Is.EqualTo(7.30m)); 

            var tpdRiderPlanResult = riskResult.PlanPremiumCalculationResults.First(p => p.PlanCode == "TPDDTH");
            Assert.That(tpdRiderPlanResult.TotalPremium, Is.EqualTo(87.62m));
            Assert.That(tpdRiderPlanResult.MultiCoverDiscount, Is.EqualTo(0));
            Assert.That(tpdRiderPlanResult.MultiPlanDiscount, Is.EqualTo(9.74m));

            var tpdRiderIllnessCoverResult = tpdRiderPlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "TPDDTHIC");
            Assert.That(tpdRiderIllnessCoverResult.TotalPremium, Is.EqualTo(87.62m));

            var ciRiderPlanResult = riskResult.PlanPremiumCalculationResults.First(p => p.PlanCode == "TRADTH");
            Assert.That(ciRiderPlanResult.TotalPremium, Is.EqualTo(35.72m));
            Assert.That(ciRiderPlanResult.MultiCoverDiscount, Is.EqualTo(21.44m));
            Assert.That(ciRiderPlanResult.MultiPlanDiscount, Is.EqualTo(0));

            var ciRiderAccCoverResult = ciRiderPlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "TRADTHSIN");
            Assert.That(ciRiderAccCoverResult.TotalPremium, Is.EqualTo(10.72m));

            var ciRiderIllnessCoverResult = ciRiderPlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "TRADTHSIC");
            Assert.That(ciRiderIllnessCoverResult.TotalPremium, Is.EqualTo(17.86m));

            var ciRiderCancerCoverResult = ciRiderPlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "TRADTHCC");
            Assert.That(ciRiderCancerCoverResult.TotalPremium, Is.EqualTo(28.58m));

        }

        [Test]
        public void Calculate_TestCaseTwo()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var lifeAccidentCover = BuildLifeAccidentCover(0, 0);
            var lifeIllnessCover = BuildLifeIllnessCover(0,0);
            var lifePlan = BuildLifePlan(500000, PremiumType.Stepped, true, new List<CoverCalculationRequest> { lifeAccidentCover, lifeIllnessCover });

            var tpdAccidentCover = BuildTpdSaAccidentCover(0, 0);
            var tpdIllnessCover = BuildTpdSaIllnessCover(0, 0);
            var tpdSportsCover = BuildTpdSaSportsCover(0);
            var tpdPlan = BuildTpdSaPlan(750000, PremiumType.Stepped, OccupationDefinition.AnyOccupation, 1, false,
                new List<CoverCalculationRequest> {tpdAccidentCover, tpdIllnessCover, tpdSportsCover});

            var risk = BuildRisk(46, Gender.Female, false, "AAA", new List<PlanCalculationRequest> { lifePlan, tpdPlan });
            var policy = new PremiumCalculationRequest(PremiumFrequency.Monthly, new List<RiskCalculationRequest> { risk }, BrandId);

            var result = _premiumCalculationService.Calculate(policy);
            var riskResult = result.RiskPremiumCalculationResults.First();

            Assert.That(riskResult.RiskPremium, Is.EqualTo(139.29m));
            Assert.That(riskResult.MultiPlanDiscount, Is.EqualTo(8.77m)); 

            var lifePlanResult = riskResult.PlanPremiumCalculationResults.First(p => p.PlanCode == "DTH");
            Assert.That(lifePlanResult.TotalPremium, Is.EqualTo(48.23m));
            Assert.That(lifePlanResult.MultiPlanDiscount, Is.EqualTo(3.05m));
            Assert.That(lifePlanResult.MultiCoverDiscount, Is.EqualTo(9.65m));

            var lifeAccCoverResult = lifePlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "DTHAC");
            Assert.That(lifeAccCoverResult.TotalPremium, Is.EqualTo(19.29m));

            var lifeIllnessCoverResult = lifePlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "DTHIC");
            Assert.That(lifeIllnessCoverResult.TotalPremium, Is.EqualTo(38.59m));

            var tpdPlanResult = riskResult.PlanPremiumCalculationResults.First(p => p.PlanCode == "TPS");
            Assert.That(tpdPlanResult.TotalPremium, Is.EqualTo(91.06m));
            Assert.That(tpdPlanResult.MultiPlanDiscount, Is.EqualTo(5.72m));
            Assert.That(tpdPlanResult.MultiCoverDiscount, Is.EqualTo(17.54m));

            var tpdAccCoverResult = tpdPlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "TPSAC");
            Assert.That(tpdAccCoverResult.TotalPremium, Is.EqualTo(43.83m));

            var tpdIllCoverResult = tpdPlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "TPSIC");
            Assert.That(tpdIllCoverResult.TotalPremium, Is.EqualTo(61.36m));

            var tpdSportsCoverResult = tpdPlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "TPSASC");
            Assert.That(tpdSportsCoverResult.TotalPremium, Is.EqualTo(3.41m));
        }

        [Test]
        public void Calculate_TestCaseThree()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var lifeIllnessCover = BuildLifeIllnessCover(0, 0);
            var lifePlan = BuildLifePlan(1000000, PremiumType.Level, true, new List<CoverCalculationRequest> { lifeIllnessCover });

            var ciRiderIllness = BuildCiRiderIllnessCover(0, 0);
            var ciRiderCancer = BuildCiRiderCancerCover(0, 0);

            var ciRiderPlan = BuildCiRiderPlan(200000, PremiumType.Level, true, true,
                new List<CoverCalculationRequest> { ciRiderIllness, ciRiderCancer });

            var risk = BuildRisk(41, Gender.Female, false, "AAA", new List<PlanCalculationRequest> { lifePlan, ciRiderPlan });
            var policy = new PremiumCalculationRequest(PremiumFrequency.Monthly, new List<RiskCalculationRequest> { risk }, BrandId);

            var result = _premiumCalculationService.Calculate(policy);
            var riskResult = result.RiskPremiumCalculationResults.First();

            Assert.That(riskResult.RiskPremium, Is.EqualTo(210.44m));
            Assert.That(riskResult.MultiPlanDiscount, Is.EqualTo(5.30m));

            var lifePlanResult = riskResult.PlanPremiumCalculationResults.First(p => p.PlanCode == "DTH");
            Assert.That(lifePlanResult.TotalPremium, Is.EqualTo(100.79m));
            Assert.That(lifePlanResult.MultiPlanDiscount, Is.EqualTo(5.30m));
            Assert.That(lifePlanResult.MultiCoverDiscount, Is.EqualTo(0m));

            var lifeIllnessCoverResult = lifePlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "DTHIC");
            Assert.That(lifeIllnessCoverResult.TotalPremium, Is.EqualTo(100.79m));

            var ciRiderPlanResult = riskResult.PlanPremiumCalculationResults.First(p => p.PlanCode == "TRADTH");
            Assert.That(ciRiderPlanResult.TotalPremium, Is.EqualTo(109.65m));
            Assert.That(ciRiderPlanResult.MultiCoverDiscount, Is.EqualTo(65.79m));

            var ciRiderIllnessCoverResult = ciRiderPlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "TRADTHSIC");
            Assert.That(ciRiderIllnessCoverResult.TotalPremium, Is.EqualTo(67.48m));

            var ciRiderCancerCoverResult = ciRiderPlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "TRADTHCC");
            Assert.That(ciRiderCancerCoverResult.TotalPremium, Is.EqualTo(107.96m));
        }

        [Test]
        public void Calculate_TestCaseFour()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var ipAccidentCover = BuildIpAccidentCover(0);
            var ipSportsCover = BuildIpSportsCover(0);

            var ipPlan = BuildIpPlan(5000, PremiumType.Level, 4, 2, true, true,
                new List<CoverCalculationRequest> {ipAccidentCover, ipSportsCover});

            var risk = BuildRisk(38, Gender.Male, false, "AA", new List<PlanCalculationRequest> { ipPlan });
            var policy = new PremiumCalculationRequest(PremiumFrequency.Monthly, new List<RiskCalculationRequest> { risk }, BrandId);
            
            var result = _premiumCalculationService.Calculate(policy);
            var riskResult = result.RiskPremiumCalculationResults.First();

            Assert.That(riskResult.RiskPremium, Is.EqualTo(139.91m));
            Assert.That(riskResult.MultiPlanDiscount, Is.EqualTo(0));

            var ipPlanResult = riskResult.PlanPremiumCalculationResults.First(p => p.PlanCode == "IP");
            Assert.That(ipPlanResult.TotalPremium, Is.EqualTo(139.91m));
            Assert.That(ipPlanResult.MultiCoverDiscount, Is.EqualTo(0m));

            var ipAccidentCoverResult = ipPlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "IPSAC");
            Assert.That(ipAccidentCoverResult.TotalPremium, Is.EqualTo(139.31m));

            var ipSportsCoverResult = ipPlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "IPSSC");
            Assert.That(ipSportsCoverResult.TotalPremium, Is.EqualTo(0.60m));

        }

        [Test]
        public void Calculate_TestCaseFive()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var lifeAccidentCover = BuildLifeAccidentCover(0, 0);
            var lifeIllnessCover = BuildLifeIllnessCover(0, 0);
            var lifeSportsCover = BuildLifeSportsCover(0);
            var lifePlan = BuildLifePlan(500000, PremiumType.Stepped, true,
                new List<CoverCalculationRequest> {lifeAccidentCover, lifeIllnessCover, lifeSportsCover });

            var risk = BuildRisk(56, Gender.Male, false, "AAA", new List<PlanCalculationRequest> { lifePlan });
            var policy = new PremiumCalculationRequest(PremiumFrequency.Monthly, new List<RiskCalculationRequest> { risk }, BrandId);

            var result = _premiumCalculationService.Calculate(policy);
            var riskResult = result.RiskPremiumCalculationResults.First();

            Assert.That(riskResult.RiskPremium, Is.EqualTo(247.39m));
            Assert.That(riskResult.MultiPlanDiscount, Is.EqualTo(0));

            var lifePlanResult = riskResult.PlanPremiumCalculationResults.First(p => p.PlanCode == "DTH");
            Assert.That(lifePlanResult.TotalPremium, Is.EqualTo(247.39m));
            Assert.That(lifePlanResult.MultiCoverDiscount, Is.EqualTo(49.00m));

            var lifeAccCoverResult = lifePlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "DTHAC");
            Assert.That(lifeAccCoverResult.TotalPremium, Is.EqualTo(97.99m));

            var lifeIllnessCoverResult = lifePlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "DTHIC");
            Assert.That(lifeIllnessCoverResult.TotalPremium, Is.EqualTo(195.97m));

            var lifeSportsCoverResult = lifePlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "DTHASC");
            Assert.That(lifeSportsCoverResult.TotalPremium, Is.EqualTo(2.43m));
        }

        [Test]
        public void Calculate_TestCaseSix()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var ciAccidentCover = BuildCiSaAccidentCover(0, 0);
            var ciCancerCover = BuildCiSaCancerCover(0, 0);

            var ciPlan = BuildCiSaPlan(250000, PremiumType.Stepped, true,
                new List<CoverCalculationRequest> {ciAccidentCover, ciCancerCover});

            var ipAccidentCover = BuildIpAccidentCover(0);
            var ipIllnessCover = BuildIpIllnessCover(0);
            var ipSportsCover = BuildIpSportsCover(0);

            var ipPlan = BuildIpPlan(7000, PremiumType.Stepped, 104, 5, false, false,
                new List<CoverCalculationRequest> { ipAccidentCover, ipIllnessCover, ipSportsCover });

            var risk = BuildRisk(43, Gender.Female, false, "AA", new List<PlanCalculationRequest> { ipPlan, ciPlan });
            var policy = new PremiumCalculationRequest(PremiumFrequency.Monthly, new List<RiskCalculationRequest> { risk }, BrandId);

            var result = _premiumCalculationService.Calculate(policy);
            var riskResult = result.RiskPremiumCalculationResults.First();

            Assert.That(riskResult.RiskPremium, Is.EqualTo(197.62m));
            Assert.That(riskResult.MultiPlanDiscount, Is.EqualTo(0));

            var ciPlanResult = riskResult.PlanPremiumCalculationResults.First(p => p.PlanCode == "TRS");
            Assert.That(ciPlanResult.TotalPremium, Is.EqualTo(78.49m));
            Assert.That(ciPlanResult.MultiCoverDiscount, Is.EqualTo(47.10m));

            var ciAccidentCoverResult = ciPlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "TRSSIN");
            Assert.That(ciAccidentCoverResult.TotalPremium, Is.EqualTo(34.25m));

            var ciCancerCoverResult = ciPlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "TRSCC");
            Assert.That(ciCancerCoverResult.TotalPremium, Is.EqualTo(91.34m));

            var ipPlanResult = riskResult.PlanPremiumCalculationResults.First(p => p.PlanCode == "IP");
            Assert.That(ipPlanResult.TotalPremium, Is.EqualTo(119.13m));
            Assert.That(ipPlanResult.MultiCoverDiscount, Is.EqualTo(23.60m));

            var ipAccidentCoverResult = ipPlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "IPSAC");
            Assert.That(ipAccidentCoverResult.TotalPremium, Is.EqualTo(58.98m));

            var ipIllnessCoverResult = ipPlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "IPSIC");
            Assert.That(ipIllnessCoverResult.TotalPremium, Is.EqualTo(82.57m));

            var ipSportsCoverResult = ipPlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "IPSSC");
            Assert.That(ipSportsCoverResult.TotalPremium, Is.EqualTo(1.18m));
        }

        [Test]
        public void Calculate_TestCaseSeven()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var lifeAccidentCover = BuildLifeAccidentCover(0, 0);
            var lifeIllnessCover = BuildLifeIllnessCover(0, 0);
            var lifePlan = BuildLifePlan(500000, PremiumType.Stepped, false,
                new List<CoverCalculationRequest> { lifeAccidentCover, lifeIllnessCover });

            var risk = BuildRisk(20, Gender.Male, false, "AA", new List<PlanCalculationRequest> { lifePlan });
            var policy = new PremiumCalculationRequest(PremiumFrequency.Monthly, new List<RiskCalculationRequest> { risk }, BrandId);

            var result = _premiumCalculationService.Calculate(policy);
            var riskResult = result.RiskPremiumCalculationResults.First();

            Assert.That(riskResult.RiskPremium, Is.EqualTo(51.37m));
            Assert.That(riskResult.MultiPlanDiscount, Is.EqualTo(0));

            var lifePlanResult = riskResult.PlanPremiumCalculationResults.First(p => p.PlanCode == "DTH");
            Assert.That(lifePlanResult.TotalPremium, Is.EqualTo(51.37m));
            Assert.That(lifePlanResult.MultiCoverDiscount, Is.EqualTo(10.28m));

            var lifeAccCoverResult = lifePlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "DTHAC");
            Assert.That(lifeAccCoverResult.TotalPremium, Is.EqualTo(20.55m));

            var lifeIllnessCoverResult = lifePlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "DTHIC");
            Assert.That(lifeIllnessCoverResult.TotalPremium, Is.EqualTo(41.10m));
        }

        [Test]
        public void Calculate_TestCaseEight()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var tpdAccidentCover = BuildTpdSaAccidentCover(50, 0);
            var tpdSportsCover = BuildTpdSaSportsCover(1);
            var tpdPlan = BuildTpdSaPlan(500000, PremiumType.Stepped, OccupationDefinition.OwnOccupation, 1, true,
                new List<CoverCalculationRequest> { tpdAccidentCover, tpdSportsCover });

            var risk = BuildRisk(25, Gender.Female, true, "AA", new List<PlanCalculationRequest> { tpdPlan });
            var policy = new PremiumCalculationRequest(PremiumFrequency.Quarterly, new List<RiskCalculationRequest> { risk }, BrandId);

            var result = _premiumCalculationService.Calculate(policy);
            var riskResult = result.RiskPremiumCalculationResults.First();

            Assert.That(riskResult.RiskPremium, Is.EqualTo(288.08m));
            Assert.That(riskResult.MultiPlanDiscount, Is.EqualTo(0));

            var tpdPlanResult = riskResult.PlanPremiumCalculationResults.First(p => p.PlanCode == "TPS");
            Assert.That(tpdPlanResult.TotalPremium, Is.EqualTo(288.08m));
            Assert.That(tpdPlanResult.MultiCoverDiscount, Is.EqualTo(0));

            var tpdAccCoverResult = tpdPlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "TPSAC");
            Assert.That(tpdAccCoverResult.BasePremium, Is.EqualTo(89.92m));
            Assert.That(tpdAccCoverResult.LoadingPremium, Is.EqualTo(44.96m));

            var tpdSportsCoverResult = tpdPlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "TPSASC");
            Assert.That(tpdSportsCoverResult.TotalPremium, Is.EqualTo(153.20));
        }

        [Test]
        public void Calculate_TestCaseNine()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var ciAccidentCover = BuildCiSaAccidentCover(20, 0.5m);

            var ciPlan = BuildCiSaPlan(500000, PremiumType.Level, false,
                new List<CoverCalculationRequest> { ciAccidentCover });

            var risk = BuildRisk(30, Gender.Male, true, "AA", new List<PlanCalculationRequest> { ciPlan });
            var policy = new PremiumCalculationRequest(PremiumFrequency.HalfYearly, new List<RiskCalculationRequest> { risk }, BrandId);

            var result = _premiumCalculationService.Calculate(policy);
            var riskResult = result.RiskPremiumCalculationResults.First();

            Assert.That(riskResult.RiskPremium, Is.EqualTo(785.26m));
            Assert.That(riskResult.MultiPlanDiscount, Is.EqualTo(0));

            var ciPlanResult = riskResult.PlanPremiumCalculationResults.First(p => p.PlanCode == "TRS");
            Assert.That(ciPlanResult.TotalPremium, Is.EqualTo(785.26m));
            Assert.That(ciPlanResult.MultiCoverDiscount, Is.EqualTo(0));

            var ciAccidentCoverResult = ciPlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "TRSSIN");
            Assert.That(ciAccidentCoverResult.BasePremium, Is.EqualTo(550.22m));
            Assert.That(ciAccidentCoverResult.LoadingPremium, Is.EqualTo(235.04m));
        }

        [Test]
        public void Calculate_TestCaseTen()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var ipAccidentCover = BuildIpAccidentCover(0);
            var ipIllnessCover = BuildIpIllnessCover(0);
            var ipSportsCover = BuildIpSportsCover(0);

            var ipPlan = BuildIpPlan(4000, PremiumType.Stepped, 2, 1, true, true,
                new List<CoverCalculationRequest> { ipAccidentCover, ipIllnessCover, ipSportsCover });

            var risk = BuildRisk(35, Gender.Female, false, "AA+", new List<PlanCalculationRequest> { ipPlan });
            var policy = new PremiumCalculationRequest(PremiumFrequency.Yearly, new List<RiskCalculationRequest> { risk }, BrandId);

            var result = _premiumCalculationService.Calculate(policy);
            var riskResult = result.RiskPremiumCalculationResults.First();

            Assert.That(riskResult.RiskPremium, Is.EqualTo(1409.90m));
            Assert.That(riskResult.MultiPlanDiscount, Is.EqualTo(0));

            var ipPlanResult = riskResult.PlanPremiumCalculationResults.First(p => p.PlanCode == "IP");
            Assert.That(ipPlanResult.TotalPremium, Is.EqualTo(1409.90m));
            Assert.That(ipPlanResult.MultiCoverDiscount, Is.EqualTo(247.63));

            var ipAccidentCoverResult = ipPlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "IPSAC");
            Assert.That(ipAccidentCoverResult.TotalPremium, Is.EqualTo(778.62m));

            var ipIllnessCoverResult = ipPlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "IPSIC");
            Assert.That(ipIllnessCoverResult.TotalPremium, Is.EqualTo(866.53m));

            var ipSportsCoverResult = ipPlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "IPSSC");
            Assert.That(ipSportsCoverResult.TotalPremium, Is.EqualTo(12.38m));
        }

        [Test]
        public void Calculate_TestCaseEleven()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var lifeAccident = BuildLifeAccidentCover(100, 0);
            var lifeSports = BuildLifeSportsCover(3);

            var lifePlan = BuildLifePlan(200000m, PremiumType.Stepped, false,
                new List<CoverCalculationRequest>() {lifeAccident, lifeSports});

            var tpdRiderAccident = BuildTpdRiderAccidentCover(50, 1);
            var tpdRiderSports = BuildTpdRiderSportsCover(2);

            var tpdRiderPlan = BuildTpdRiderPlan(200000m, PremiumType.Stepped, true, OccupationDefinition.AnyOccupation,
                1.5m, false, new List<CoverCalculationRequest> {tpdRiderAccident, tpdRiderSports});

            var risk = BuildRisk(40, Gender.Male, true, "AA+", new List<PlanCalculationRequest> { lifePlan, tpdRiderPlan });
            var policy = new PremiumCalculationRequest(PremiumFrequency.Monthly, new List<RiskCalculationRequest> { risk }, BrandId);

            var result = _premiumCalculationService.Calculate(policy);
            var riskResult = result.RiskPremiumCalculationResults.First();

            Assert.That(riskResult.RiskPremium, Is.EqualTo(175.99m));
            Assert.That(riskResult.MultiPlanDiscount, Is.EqualTo(6.87));

            var lifePlanResult = riskResult.PlanPremiumCalculationResults.First(p => p.PlanCode == "DTH");
            Assert.That(lifePlanResult.TotalPremium, Is.EqualTo(89.29m));
            Assert.That(lifePlanResult.MultiPlanDiscount, Is.EqualTo(3.81m));
            Assert.That(lifePlanResult.MultiCoverDiscount, Is.EqualTo(0));

            var lifeAccidentCoverResult = lifePlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "DTHAC");
            Assert.That(lifeAccidentCoverResult.BasePremium, Is.EqualTo(16.92m));
            Assert.That(lifeAccidentCoverResult.LoadingPremium, Is.EqualTo(16.92m));

            var lifeSportsCoverResult = lifePlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "DTHASC");
            Assert.That(lifeSportsCoverResult.TotalPremium, Is.EqualTo(55.45m));

            var tpdRiderPlanResult = riskResult.PlanPremiumCalculationResults.First(p => p.PlanCode == "TPDDTH");
            Assert.That(tpdRiderPlanResult.TotalPremium, Is.EqualTo(86.70m));
            Assert.That(tpdRiderPlanResult.MultiPlanDiscount, Is.EqualTo(3.06m));
            Assert.That(tpdRiderPlanResult.MultiCoverDiscount, Is.EqualTo(0));

            var tpdRiderAccidentCoverResult = tpdRiderPlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "TPDDTHAC");
            Assert.That(tpdRiderAccidentCoverResult.BasePremium, Is.EqualTo(20.83m));
            Assert.That(tpdRiderAccidentCoverResult.LoadingPremium, Is.EqualTo(28.60m));

            var tpdRiderSportsCoverResult = tpdRiderPlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "TPDDTHASC");
            Assert.That(tpdRiderSportsCoverResult.TotalPremium, Is.EqualTo(37.27m));
        }

        [Test]
        public void Calculate_TestCaseTwelve()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var lifeAccident = BuildLifeAccidentCover(0, 0);
            var lifeIllness = BuildLifeIllnessCover(50, 0);
            var lifeSports = BuildLifeSportsCover(0);

            var lifePlan = BuildLifePlan(500000m, PremiumType.Stepped, true,
                new List<CoverCalculationRequest>() { lifeAccident, lifeIllness, lifeSports });

            var ciRiderIllness = BuildCiRiderIllnessCover(20, 1);

            var ciRiderPlan = BuildCiRiderPlan(200000m, PremiumType.Stepped, false, true, new List<CoverCalculationRequest> { ciRiderIllness });

            var risk = BuildRisk(45, Gender.Female, false, "AA+", new List<PlanCalculationRequest> { lifePlan, ciRiderPlan });
            var policy = new PremiumCalculationRequest(PremiumFrequency.Quarterly, new List<RiskCalculationRequest> { risk }, BrandId);

            var result = _premiumCalculationService.Calculate(policy);
            var riskResult = result.RiskPremiumCalculationResults.First();

            Assert.That(riskResult.RiskPremium, Is.EqualTo(423.72m));
            Assert.That(riskResult.MultiPlanDiscount, Is.EqualTo(8.39m));

            var lifePlanResult = riskResult.PlanPremiumCalculationResults.First(p => p.PlanCode == "DTH");
            Assert.That(lifePlanResult.TotalPremium, Is.EqualTo(197.35m));
            Assert.That(lifePlanResult.MultiPlanDiscount, Is.EqualTo(8.39m));
            Assert.That(lifePlanResult.MultiCoverDiscount, Is.EqualTo(25.35m));

            var lifeAccidentCoverResult = lifePlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "DTHAC");
            Assert.That(lifeAccidentCoverResult.BasePremium, Is.EqualTo(50.69m));
            Assert.That(lifeAccidentCoverResult.LoadingPremium, Is.EqualTo(0));

            var lifeIllnessCoverResult = lifePlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "DTHIC");
            Assert.That(lifeIllnessCoverResult.BasePremium, Is.EqualTo(101.36m));
            Assert.That(lifeIllnessCoverResult.LoadingPremium, Is.EqualTo(63.35m));

            var lifeSportsCoverResult = lifePlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "DTHASC");
            Assert.That(lifeSportsCoverResult.TotalPremium, Is.EqualTo(7.30m));

            var ciRiderPlanResult = riskResult.PlanPremiumCalculationResults.First(p => p.PlanCode == "TRADTH");
            Assert.That(ciRiderPlanResult.TotalPremium, Is.EqualTo(226.37m));
            Assert.That(ciRiderPlanResult.MultiPlanDiscount, Is.EqualTo(0));
            Assert.That(ciRiderPlanResult.MultiCoverDiscount, Is.EqualTo(0));

            var ciRiderIllnessCoverResult = ciRiderPlanResult.CoverPremiumCalculationResults.First(c => c.CoverCode == "TRADTHSIC");
            Assert.That(ciRiderIllnessCoverResult.BasePremium, Is.EqualTo(140.01m));
            Assert.That(ciRiderIllnessCoverResult.LoadingPremium, Is.EqualTo(86.36m));
        }

        [Test]
        public void Calculate_Defect_SixPlansRequested_MultiPlanDiscountCappedAtFour()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var tpdAccidentCover = BuildTpdSaAccidentCover(50, 0);
            var tpdPlan = BuildTpdSaPlan(500000, PremiumType.Stepped, OccupationDefinition.OwnOccupation, 1, true,
                new List<CoverCalculationRequest> { tpdAccidentCover });

            var ciAccidentCover = BuildCiSaAccidentCover(0, 0);

            var ciPlan = BuildCiSaPlan(250000, PremiumType.Stepped, true,
                new List<CoverCalculationRequest> { ciAccidentCover });

            var ipAccidentCover = BuildIpAccidentCover(0);

            var ipPlan = BuildIpPlan(7000, PremiumType.Stepped, 104, 5, false, false,
                new List<CoverCalculationRequest> { ipAccidentCover });

            var lifeAccidentCover = BuildLifeAccidentCover(0, 0);
            var lifePlan = BuildLifePlan(500000, PremiumType.Stepped, false,
                new List<CoverCalculationRequest> { lifeAccidentCover });

            var tpdRiderIllness = BuildTpdRiderIllnessCover(0, 0);
            var tpdRiderPlan = BuildTpdRiderPlan(1000000, PremiumType.Stepped, false, OccupationDefinition.AnyOccupation, 2, true, new List<CoverCalculationRequest> { tpdRiderIllness });

            var ciRiderAccident = BuildCiRiderAccidentCover(0, 0);

            var ciRiderPlan = BuildCiRiderPlan(100000, PremiumType.Stepped, false, true,
                new List<CoverCalculationRequest> { ciRiderAccident });

            var risk = BuildRisk(43, Gender.Female, false, "AA", new List<PlanCalculationRequest> { ipPlan, ciPlan, tpdPlan, lifePlan, tpdRiderPlan, ciRiderPlan });
            var policy = new PremiumCalculationRequest(PremiumFrequency.Monthly, new List<RiskCalculationRequest> { risk }, BrandId);

            var result = _premiumCalculationService.Calculate(policy);
            var riskResult = result.RiskPremiumCalculationResults.First();
        }

        //life
        private CoverCalculationRequest BuildLifeAccidentCover(decimal percentageLoading, decimal permilleLoading)
        {
            return new CoverCalculationRequest("DTHAC", true, true, true, true, new Loadings(percentageLoading, permilleLoading), BrandId);
        }

        private CoverCalculationRequest BuildLifeIllnessCover(decimal percentageLoading, decimal permilleLoading)
        {
            return new CoverCalculationRequest("DTHIC", true, true, true, true, new Loadings(percentageLoading, permilleLoading), BrandId);
        }

        private CoverCalculationRequest BuildLifeSportsCover(decimal permilleLoading)
        {
            return new CoverCalculationRequest("DTHASC", true, false, false, true, new Loadings(0, permilleLoading), BrandId);
        }

        //tpd rider
        private CoverCalculationRequest BuildTpdRiderAccidentCover(decimal percentageLoading, decimal permilleLoading)
        {
            return new CoverCalculationRequest("TPDDTHAC", true, true, true, true, new Loadings(percentageLoading, permilleLoading), BrandId);
        }

        private CoverCalculationRequest BuildTpdRiderIllnessCover(decimal percentageLoading, decimal permilleLoading)
        {
            return new CoverCalculationRequest("TPDDTHIC", true, true, true, true, new Loadings(percentageLoading, permilleLoading), BrandId);
        }

        private CoverCalculationRequest BuildTpdRiderSportsCover(decimal permilleLoading)
        {
            return new CoverCalculationRequest("TPDDTHASC", true, false, false, true, new Loadings(0, permilleLoading), BrandId);
        }

        //ci rider
        private CoverCalculationRequest BuildCiRiderAccidentCover(decimal percentageLoading, decimal permilleLoading)
        {
            return new CoverCalculationRequest("TRADTHSIN", true, true, true, true, new Loadings(percentageLoading, permilleLoading), BrandId);
        }

        private CoverCalculationRequest BuildCiRiderIllnessCover(decimal percentageLoading, decimal permilleLoading)
        {
            return new CoverCalculationRequest("TRADTHSIC", true, true, true, true, new Loadings(percentageLoading, permilleLoading), BrandId);
        }

        private CoverCalculationRequest BuildCiRiderCancerCover(decimal percentageLoading, decimal permilleLoading)
        {
            return new CoverCalculationRequest("TRADTHCC", true, true, true, true, new Loadings(percentageLoading, permilleLoading), BrandId);
        }

        //tpd SA
        private CoverCalculationRequest BuildTpdSaAccidentCover(decimal percentageLoading, decimal permilleLoading)
        {
            return new CoverCalculationRequest("TPSAC", true, true, true, true, new Loadings(percentageLoading, permilleLoading), BrandId);
        }

        private CoverCalculationRequest BuildTpdSaIllnessCover(decimal percentageLoading, decimal permilleLoading)
        {
            return new CoverCalculationRequest("TPSIC", true, true, true, true, new Loadings(percentageLoading, permilleLoading), BrandId);
        }

        private CoverCalculationRequest BuildTpdSaSportsCover(decimal permilleLoading)
        {
            return new CoverCalculationRequest("TPSASC", true, false, false, true, new Loadings(0, permilleLoading), BrandId);
        }

        //ci SA
        private CoverCalculationRequest BuildCiSaAccidentCover(decimal percentageLoading, decimal permilleLoading)
        {
            return new CoverCalculationRequest("TRSSIN", true, true, true, true, new Loadings(percentageLoading, permilleLoading), BrandId);
        }

        private CoverCalculationRequest BuildCiSaIllnessCover(decimal percentageLoading, decimal permilleLoading)
        {
            return new CoverCalculationRequest("TRSSIC", true, true, true, true, new Loadings(percentageLoading, permilleLoading), BrandId);
        }

        private CoverCalculationRequest BuildCiSaCancerCover(decimal percentageLoading, decimal permilleLoading)
        {
            return new CoverCalculationRequest("TRSCC", true, true, true, true, new Loadings(percentageLoading, permilleLoading), BrandId);
        }

        //ip
        private CoverCalculationRequest BuildIpAccidentCover(decimal percentageLoading)
        {
            return new CoverCalculationRequest("IPSAC", true, true, true, false, new Loadings(percentageLoading, 0), BrandId);
        }

        private CoverCalculationRequest BuildIpIllnessCover(decimal percentageLoading)
        {
            return new CoverCalculationRequest("IPSIC", true, true, true, false, new Loadings(percentageLoading, 0), BrandId);
        }

        private CoverCalculationRequest BuildIpSportsCover(decimal percentageLoading)
        {
            return new CoverCalculationRequest("IPSSC", true, false, true, false, new Loadings(percentageLoading, 0), BrandId);
        }

        //plans

        private PlanCalculationRequest BuildLifePlan(decimal coverAmount, PremiumType premiumType, bool premiumRelief, IReadOnlyList<CoverCalculationRequest> covers)
        {
            return new PlanCalculationRequest("DTH", true, true, coverAmount, premiumType, null, null, null, OccupationDefinition.Unknown, null, premiumRelief, null, null, covers);
        }

        private PlanCalculationRequest BuildTpdSaPlan(decimal coverAmount, PremiumType premiumType, OccupationDefinition occupationDefinition, decimal occLoading, bool premiumRelief, IReadOnlyList<CoverCalculationRequest> covers)
        {
            return new PlanCalculationRequest("TPS", true, true, coverAmount, premiumType, null, null, null, occupationDefinition, occLoading, premiumRelief, null, null, covers);
        }

        private PlanCalculationRequest BuildTpdRiderPlan(decimal coverAmount, PremiumType premiumType, bool buyBack, OccupationDefinition occupationDefinition, decimal occLoading, bool premiumRelief, IReadOnlyList<CoverCalculationRequest> covers)
        {
            return new PlanCalculationRequest("TPDDTH", true, true, coverAmount, premiumType, buyBack, null, null, occupationDefinition, occLoading, premiumRelief, null, null, covers);
        }

        private PlanCalculationRequest BuildCiSaPlan(decimal coverAmount, PremiumType premiumType, bool premiumRelief, IReadOnlyList<CoverCalculationRequest> covers)
        {
            return new PlanCalculationRequest("TRS", true, false, coverAmount, premiumType, null, null, null, OccupationDefinition.Unknown, null, premiumRelief, null, null, covers);
        }

        private PlanCalculationRequest BuildCiRiderPlan(decimal coverAmount, PremiumType premiumType, bool buyBack, bool premiumRelief, IReadOnlyList<CoverCalculationRequest> covers )
        {
            return new PlanCalculationRequest("TRADTH", true, false, coverAmount, premiumType, buyBack, null, null, OccupationDefinition.Unknown, null, premiumRelief, null, null, covers);
        }

        private PlanCalculationRequest BuildIpPlan(decimal coverAmount, PremiumType premiumType, int waitingPeriod, int benefitPeriod, bool increasingClaims, bool dayOneAccident, IReadOnlyList<CoverCalculationRequest> covers)
        {
            return new PlanCalculationRequest("IP", true, false, coverAmount, premiumType, null, waitingPeriod, benefitPeriod, OccupationDefinition.Unknown, null, null, increasingClaims, dayOneAccident, covers);
        }

        //risk
        private RiskCalculationRequest BuildRisk(int ageNextBirthday, Gender gender, bool isSmoker, string occClass, IReadOnlyList<PlanCalculationRequest> plans)
        {
            return new RiskCalculationRequest(1, ageNextBirthday, gender, isSmoker, occClass, plans);
        }
    }
}
