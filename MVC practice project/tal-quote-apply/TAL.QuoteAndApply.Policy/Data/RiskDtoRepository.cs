using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Infrastructure;
using TAL.QuoteAndApply.Infrastructure.Cache;
using TAL.QuoteAndApply.Infrastructure.Observer;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Events;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Data
{
    public interface IRiskDtoRepository
    {
        RiskDto GetRisk(int riskId, bool ignoreCache = false);
        RiskDto GetRiskByInterviewId(string interviewId);
        RiskDto GetRiskByPartyId(int partyId);
        RiskDto InsertRisk(RiskDto risk);
        void UpdateRisk(RiskDto risk);
        void SetLatestConcurrencyToken(RiskDto risk, string concurrencyToken);
        IEnumerable<RiskDto> GetRisksForPolicy(int policyId);
    }

    public class RiskDtoRepository : BaseRepository<RiskDto>, IRiskDtoRepository
    {
        private readonly ICachingWrapper _cachingWrapper;
        private readonly IRiskChangeSubject _riskChangeSubject;

        public RiskDtoRepository(IPolicyConfigurationProvider settings, ICurrentUserProvider currentUserProvider,
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService, ICachingWrapper cachingWrapper, IRiskChangeSubject riskChangeSubject)
            : base(settings.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
            _cachingWrapper = cachingWrapper;
            _riskChangeSubject = riskChangeSubject;
        }

        public RiskDto GetRisk(int id, bool ignoreCache = false)
        {
            if (ignoreCache)
            {
                return _cachingWrapper.UpdateOrAddCacheItemRequestScope(GetType(), $"RiskId-{id}", Get(id));
            }
            return _cachingWrapper.GetOrAddCacheItemRequestScope(GetType(), $"RiskId-{id}", () => Get(id));
        }

        public RiskDto GetRiskByInterviewId(string interviewId) //
        {
            var risks = _cachingWrapper.GetOrAddCacheItemRequestScope(GetType(), $"InterviewId-{interviewId}",
                () => Where(risk => risk.InterviewId, Op.Eq, interviewId));
            return risks.Select(r => GetRisk(r.Id)).FirstOrDefault();
        }

        public RiskDto GetRiskByPartyId(int partyId) //
        {
            var risks = _cachingWrapper.GetOrAddCacheItemRequestScope(GetType(), $"PartyId-{partyId}",
                () => Where(risk => risk.PartyId, Op.Eq, partyId));
            return risks.Select(r => GetRisk(r.Id)).FirstOrDefault();
        }

        public RiskDto InsertRisk(RiskDto risk)
        {
            var newDto = Insert(risk);
            return _cachingWrapper.UpdateOrAddCacheItemRequestScope(GetType(), $"RiskId-{risk.Id}", newDto);
        }

        public void UpdateRisk(RiskDto risk)
        {
            Update(risk);
            var updatedDto = Get(risk.Id);
            risk.RV = updatedDto.RV;
            _cachingWrapper.UpdateOrAddCacheItemRequestScope(GetType(), $"RiskId-{risk.Id}", risk);

            _riskChangeSubject.Notify(new ChangeEnvelope(risk));
        }

        public void SetLatestConcurrencyToken(RiskDto risk, string concurrencyToken)
        {
            risk.InterviewConcurrencyToken = concurrencyToken;
            UpdateRisk(risk);
        }

        public IEnumerable<RiskDto> GetRisksForPolicy(int policyId)
        {
            var risks = _cachingWrapper.GetOrAddCacheItemRequestScope(GetType(), $"PolicyId-{policyId}",
                () => Where(risk => risk.PolicyId, Op.Eq, policyId));

            return risks.Select(r => GetRisk(r.Id));
        }

        public new bool Delete(RiskDto risk)
        {
            var result = base.Delete(risk);

            if (result)
            {
                _cachingWrapper.RemoveItem(GetType(), $"RiskId-{risk.Id}");
            }

            return result;
        }
    }

}
