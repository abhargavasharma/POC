using System;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Infrastructure.Time;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Service.AccessControl
{
    public interface IPolicyAccessControlService
    {
        void UpdatePolicyAccessControlByPolicyId(int policyId, string userName, AccessControlType userType);
        void UpdatePolicyAccessControlByRisk(IRisk risk, string userName, AccessControlType userType);
        void UpdatePolicyAccessControlByRiskOccupation(IOccupationRatingFactors occupationRatingFactors, string userName, AccessControlType userType);
        void UpdatePolicyAccessControlByCover(ICover cover, string userName, AccessControlType userType);
        void UpdatePolicyAccessControlByCoverLoading(ICoverLoading cover, string userName, AccessControlType userType);
        void UpdatePolicyAccessControlByPlan(IPlan plan, string userName, AccessControlType userType);
        void UpdatePolicyAccessControlByOption(IOption option, string userName, AccessControlType userType);
        void UpdatePolicyAccessControlByCoverExclusion(ICoverExclusion coverExclusion, string userName, AccessControlType userType);
    }

    public class PolicyAccessControlService : IPolicyAccessControlService
    {
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IPolicyAccessControlDtoRepository _policyAccessControlDtoRepository;
        private readonly ICachingWrapper _cachingWrapper;

        private readonly IRiskDtoRepository _riskDtoRepository;
        private readonly ICoverDtoRepository _coverDtoRepository;

        public PolicyAccessControlService(IPolicyAccessControlDtoRepository policyAccessControlDtoRepository,
            IDateTimeProvider dateTimeProvider, ICachingWrapper cachingWrapper, IRiskDtoRepository riskDtoRepository,
            ICoverDtoRepository coverDtoRepository)
        {
            _policyAccessControlDtoRepository = policyAccessControlDtoRepository;
            _dateTimeProvider = dateTimeProvider;
            _cachingWrapper = cachingWrapper;
            _riskDtoRepository = riskDtoRepository;
            _coverDtoRepository = coverDtoRepository;
        }

        public void UpdatePolicyAccessControlByPolicyId(int policyId, string userName, AccessControlType userType)
        {
            var accessItem = _policyAccessControlDtoRepository.GetAccessControlForPolicy(policyId);
            accessItem.LastTouchedByName = userName;
            accessItem.LastTouchedByType = userType;
            accessItem.LastTouchedTime = _dateTimeProvider.GetCurrentDateAndTime();
            _policyAccessControlDtoRepository.UpdateAccessControl(accessItem);
        }

        public void UpdatePolicyAccessControlByRisk(IRisk risk, string userName, AccessControlType userType)
        {
            UpdatePolicyAccessControlByPolicyId(risk.PolicyId, userName, userType);
        }

        public void UpdatePolicyAccessControlByRiskOccupation(IOccupationRatingFactors occupationRatingFactors, string userName,
            AccessControlType userType)
        {
            var risk = _cachingWrapper.GetOrAddCacheItemSliding(GetType(), $"RiskId-{occupationRatingFactors.Id}", TimeSpan.FromMinutes(5),
                () => _riskDtoRepository.GetRisk(occupationRatingFactors.Id));

            UpdatePolicyAccessControlByPolicyId(risk.PolicyId, userName, userType);
        }

        public void UpdatePolicyAccessControlByCover(ICover cover, string userName, AccessControlType userType)
        {
            var risk = _cachingWrapper.GetOrAddCacheItemSliding(GetType(), $"CoverId-{cover.Id}", TimeSpan.FromMinutes(5),
                () => _riskDtoRepository.GetRisk(cover.RiskId));

            UpdatePolicyAccessControlByPolicyId(risk.PolicyId, userName, userType);
        }

        public void UpdatePolicyAccessControlByCoverLoading(ICoverLoading cover, string userName, AccessControlType userType)
        {
            var risk = _cachingWrapper.GetOrAddCacheItemSliding(GetType(), $"CoverId-{cover.CoverId}", TimeSpan.FromMinutes(5),
                () => _riskDtoRepository.GetRisk(_coverDtoRepository.GetCover(cover.CoverId).RiskId));

            UpdatePolicyAccessControlByPolicyId(risk.PolicyId, userName, userType);
        }

        public void UpdatePolicyAccessControlByPlan(IPlan plan, string userName, AccessControlType userType)
        {
            var risk = _cachingWrapper.GetOrAddCacheItemSliding(GetType(), $"PlanId-{plan.Id}", TimeSpan.FromMinutes(5),
                () => _riskDtoRepository.GetRisk(plan.RiskId));

            UpdatePolicyAccessControlByPolicyId(risk.PolicyId, userName, userType);
        }

        public void UpdatePolicyAccessControlByOption(IOption option, string userName, AccessControlType userType)
        {
            var risk = _cachingWrapper.GetOrAddCacheItemSliding(GetType(), $"OptionId-{option.Id}", TimeSpan.FromMinutes(5),
                () => _riskDtoRepository.GetRisk(option.RiskId));

            UpdatePolicyAccessControlByPolicyId(risk.PolicyId, userName, userType);
        }

        public void UpdatePolicyAccessControlByCoverExclusion(ICoverExclusion coverExclusion, string userName,
            AccessControlType userType)
        {
            var risk = _cachingWrapper.GetOrAddCacheItemSliding(GetType(), $"CoverId-{coverExclusion.CoverId}", TimeSpan.FromMinutes(5),
                () => _riskDtoRepository.GetRisk(_coverDtoRepository.GetCover(coverExclusion.CoverId).RiskId));

            UpdatePolicyAccessControlByPolicyId(risk.PolicyId, userName, userType);
        }
    }
}
