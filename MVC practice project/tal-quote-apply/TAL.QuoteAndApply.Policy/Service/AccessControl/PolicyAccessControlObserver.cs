using System;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Events;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Service.AccessControl
{
    public class PolicyAccessControlObserver : ICoverChangeObserver, IRiskChangeObserver, IPlanChangeObserver, IOptionChangeObserver, ICoverLoadingChangeObserver, IRiskOccupationChangeObserver, ICoverExclusionChangeObserver
    {
        private readonly IPolicyAccessControlService _policyAccessControlService;
        private readonly ICurrentUserProvider _currentUserProvider;
        private readonly IAccessControlTypeProvider _accessControlTypeProvider;

        public PolicyAccessControlObserver(IPolicyAccessControlService policyAccessControlService,
            ICurrentUserProvider currentUserProvider, IAccessControlTypeProvider accessControlTypeProvider)
        {
            _policyAccessControlService = policyAccessControlService;
            _currentUserProvider = currentUserProvider;
            _accessControlTypeProvider = accessControlTypeProvider;
        }

        public void Update(ICover subjectChange)
        {
            var user =_currentUserProvider.GetForApplication();
            var accessType = _accessControlTypeProvider.GetFor(user.Roles);
            _policyAccessControlService.UpdatePolicyAccessControlByCover(subjectChange, user.UserName, accessType);
        }


        public void Update(IRisk subjectChange)
        {
            var user = _currentUserProvider.GetForApplication();
            var accessType = _accessControlTypeProvider.GetFor(user.Roles);
            _policyAccessControlService.UpdatePolicyAccessControlByRisk(subjectChange, user.UserName, accessType);
        }

        public void Update(IPlan subjectChange)
        {
            var user = _currentUserProvider.GetForApplication();
            var accessType = _accessControlTypeProvider.GetFor(user.Roles);
            _policyAccessControlService.UpdatePolicyAccessControlByPlan(subjectChange, user.UserName, accessType);
        }

        public void Update(IOption subjectChange)
        {
            var user = _currentUserProvider.GetForApplication();
            var accessType = _accessControlTypeProvider.GetFor(user.Roles);
            _policyAccessControlService.UpdatePolicyAccessControlByOption(subjectChange, user.UserName, accessType);
        }

        public void Update(ICoverLoading subjectChange)
        {
            var user = _currentUserProvider.GetForApplication();
            var accessType = _accessControlTypeProvider.GetFor(user.Roles);
            _policyAccessControlService.UpdatePolicyAccessControlByCoverLoading(subjectChange, user.UserName, accessType);
        }

        public void Update(IOccupationRatingFactors subjectChange)
        {
            var user = _currentUserProvider.GetForApplication();
            var accessType = _accessControlTypeProvider.GetFor(user.Roles);
            _policyAccessControlService.UpdatePolicyAccessControlByRiskOccupation(subjectChange, user.UserName, accessType);
        }

        public void Update(ICoverExclusion subjectChange)
        {
            var user = _currentUserProvider.GetForApplication();
            var accessType = _accessControlTypeProvider.GetFor(user.Roles);
            _policyAccessControlService.UpdatePolicyAccessControlByCoverExclusion(subjectChange, user.UserName, accessType);
        }
    }
}