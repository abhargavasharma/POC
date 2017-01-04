using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataLayer.Factory;
using TAL.QuoteAndApply.DataLayer.Repository;
using TAL.QuoteAndApply.DataLayer.Repository.PredicateLogic;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Data
{
    public interface IReferralDtoRepository
    {
        ReferralDto GetById(int referralId);
        ReferralDto CreateReferral(ReferralDto referralDto);
        ReferralDto GetForPolicy(int policyId);
        ReferralDto GetInprogressReferralForPolicy(int policyId);
        void UpdateReferral(ReferralDto referralDto);
        IEnumerable<ReferralDto> GetAllReferrals();
        IEnumerable<ReferralDto> GetCompletedReferralsForPolicy(int policyId);
    }

    public class ReferralDtoRepository : BaseRepository<ReferralDto>, IReferralDtoRepository
    {
        public ReferralDtoRepository(IPolicyConfigurationProvider policyConfigurationProvider, ICurrentUserProvider currentUserProvider, 
            IDataLayerExceptionFactory dataLayerExceptionFactory, IDbItemEncryptionService dbItemEncryptionService) 
            : base(policyConfigurationProvider.ConnectionString, currentUserProvider, dataLayerExceptionFactory, dbItemEncryptionService)
        {
        }

        public ReferralDto GetById(int referralId)
        {
            return Where(r => r.Id, Op.Eq, referralId).FirstOrDefault();
        }

        public ReferralDto CreateReferral(ReferralDto referralDto)
        {
            return Insert(referralDto);
        }

        public ReferralDto GetForPolicy(int policyId)
        {
            return Where(r => r.PolicyId, Op.Eq, policyId).FirstOrDefault();
        }

        public ReferralDto GetInprogressReferralForPolicy(int policyId)
        {
            var q = DbItemPredicate<ReferralDto>.Where(r => r.PolicyId, Op.Eq, policyId)
                .And(r => r.IsCompleted, Op.Eq, false);

            return Query(q).SingleOrDefault();
        }

        public IEnumerable<ReferralDto> GetCompletedReferralsForPolicy(int policyId)
        {
            var q = DbItemPredicate<ReferralDto>.Where(r => r.PolicyId, Op.Eq, policyId)
                .And(r => r.IsCompleted, Op.Eq, true);

            return Query(q);
        }

        public void UpdateReferral(ReferralDto referralDto)
        {
            Update(referralDto);
        }

        public IEnumerable<ReferralDto> GetAllReferrals()
        {
            return GetAll();
        }
    }
}
