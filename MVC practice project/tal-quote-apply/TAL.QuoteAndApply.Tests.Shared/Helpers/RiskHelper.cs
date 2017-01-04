using System.Collections.Generic;
using TAL.QuoteAndApply.DataLayer.Encrpytion;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Events;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Risk;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.Tests.Shared.Mocks;

namespace TAL.QuoteAndApply.Tests.Shared.Helpers
{
    public static class RiskHelper
    {
        private static readonly RiskService _riskService;
        private static readonly RiskDtoRepository _riskDtoRepository;
        private static readonly RiskOccupationDtoRepository _riskOccupationDtoRepository;
        private static readonly PolicyConfigurationProvider _policyConfigurationProvider;
        private static readonly MockCurrentUserProvider _mockCurrentUserProvider;
        private static readonly RiskOccupationService _riskOccupationService;

        static RiskHelper()
        {
            _policyConfigurationProvider = new PolicyConfigurationProvider();
            _mockCurrentUserProvider = new MockCurrentUserProvider();
            _riskDtoRepository = GetRiskDtoRepository();
            _riskOccupationDtoRepository = GetRiskOccupationDtoRepository();
            _riskOccupationService = new RiskOccupationService(_riskOccupationDtoRepository);
            _riskService =
                new RiskService(
                    _riskDtoRepository,
                    new RiskRulesService(new RiskRulesFactory()),
                    new BeneficiaryDtoRepository(_policyConfigurationProvider, _mockCurrentUserProvider,
                        new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService())), null,
                    _riskOccupationService);
        }

        public static RiskDtoRepository GetRiskDtoRepository()
        {
            return new RiskDtoRepository(_policyConfigurationProvider, _mockCurrentUserProvider,
                new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()), new RiskChangeSubject());
        }

        public static RiskOccupationDtoRepository GetRiskOccupationDtoRepository()
        {
            return new RiskOccupationDtoRepository(_policyConfigurationProvider, _mockCurrentUserProvider,
                new DataLayerExceptionFactory(), new DbItemEncryptionService(new SimpleEncryptionService()), new CachingWrapper(new MockHttpProvider()));
        }

        public static IRisk CreateRisk(IRisk risk)
        {
            return _riskService.CreateRisk(risk);
        }

        public static List<OccupationDefinition> AvailableDefinitions(IRisk risk)
        {
            return _riskOccupationService.GetAvailableDefinitions(risk);
        }

        public static IRisk GetRisk(int riskId, bool ignoreCache)
        {
            var risk = _riskDtoRepository.GetRisk(riskId, ignoreCache);
            var occupation = _riskOccupationDtoRepository.GetForRisk(risk.Id, ignoreCache);
            risk.AssignOccupationProperties(occupation);

            return risk;
        }

        public static IRiskService GetRiskService()
        {
            return _riskService;
        }
    }
}