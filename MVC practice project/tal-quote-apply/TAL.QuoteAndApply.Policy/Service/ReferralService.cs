using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Service
{
    public interface IReferralService
    {
        IReferral CreateReferral(int policyId);
        IReferral GetInprogressReferralForPolicy(int policyId);
        IReferral CompleteReferral(int policyId, string completedByUser, DateTime completedDateTime);
        IEnumerable<IReferral> GetAll();
        IReferral AssignReferralToUnderwriter(int policyId, string userName, DateTime assignedToDateTime);
        IReferral GetLastCompletedReferralForPolicy(int policyId);
    }

    public class ReferralService : IReferralService
    {
        private readonly IReferralDtoRepository _referralDtoRepository;

        public ReferralService(IReferralDtoRepository referralDtoRepository)
        {
            _referralDtoRepository = referralDtoRepository;
        }

        public IReferral CreateReferral(int policyId)
        {
            var referral = new ReferralDto
            {
                PolicyId = policyId
            };

            return _referralDtoRepository.CreateReferral(referral);
        }

        public IReferral GetInprogressReferralForPolicy(int policyId)
        {
            return _referralDtoRepository.GetInprogressReferralForPolicy(policyId);
            
        }

        public IReferral CompleteReferral(int policyId, string completedByUser, DateTime completedDateTime)
        {
            var referral = _referralDtoRepository.GetInprogressReferralForPolicy(policyId);

            if (referral != null)
            {
                referral.IsCompleted = true;
                referral.CompletedTS = completedDateTime;
                referral.CompletedBy = completedByUser;

                _referralDtoRepository.UpdateReferral(referral);

                return _referralDtoRepository.GetById(referral.Id);
            }
            return null;
        }

        public IReferral AssignReferralToUnderwriter(int policyId, string userName, DateTime assignedToDateTime)
        {
            var referral = _referralDtoRepository.GetInprogressReferralForPolicy(policyId);

            if (referral != null)
            {
                referral.AssignedTo = userName;
                referral.AssignedToTS = assignedToDateTime;

                _referralDtoRepository.UpdateReferral(referral);
                return _referralDtoRepository.GetById(referral.Id);
            }
            return null;
        }

        public IReferral GetLastCompletedReferralForPolicy(int policyId)
        {
            var completedReferrals = _referralDtoRepository.GetCompletedReferralsForPolicy(policyId);
            return completedReferrals.OrderByDescending(x => x.CompletedTS).FirstOrDefault();
        }

        public IEnumerable<IReferral> GetAll()
        {
            return _referralDtoRepository.GetAllReferrals();
        }
    }
}
