using System;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Rules.Retrieval;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Product;
using TAL.QuoteAndApply.UserRoles.Customer;

namespace TAL.QuoteAndApply.ServiceLayer.Policy
{
    public enum PolicyRetrievalStatus
    {
        CanBeRetrieved,
        NotSaved,
        AlreadySubmitted,
        ReferredToUnderwriter,
        QuoteReferenceNotFound,
        InvalidPassword,
        HasInvalidStatus,
        LockedOutDueToRefer,
        InvalidBrand
    }

    public interface IPolicyRetrievalStatusProvider
    {
        PolicyRetrievalStatus GetFrom(string quoteReference, string password);
    }

    public class PolicyRetrievalStatusProvider : IPolicyRetrievalStatusProvider
    {
        private readonly IPolicyService _policyService;
        private readonly ICustomerAuthenticationService _customerAuthenticationService;
        private readonly IPolicyRetrievalRulesFactory _policyRetrievalRules;
        private readonly ICurrentProductBrandProvider _productBrandProvider;

        public PolicyRetrievalStatusProvider(IPolicyService policyService,
            ICustomerAuthenticationService customerAuthenticationService,
            IPolicyRetrievalRulesFactory policyRetrievalRules, ICurrentProductBrandProvider productBrandProvider)
        {
            _policyService = policyService;
            _customerAuthenticationService = customerAuthenticationService;
            _policyRetrievalRules = policyRetrievalRules;
            _productBrandProvider = productBrandProvider;
        }

        public PolicyRetrievalStatus GetFrom(string quoteReference, string password)
        {

            var policy = _policyService.GetByQuoteReferenceNumber(quoteReference);
            if (policy == null)
            {
                return PolicyRetrievalStatus.QuoteReferenceNotFound;
            }

            //check if policy was saved against different brand
            if (
                !policy.BrandKey.Equals(_productBrandProvider.GetCurrent().BrandCode,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return PolicyRetrievalStatus.InvalidBrand;
            }


            //Check if policy was actually saved
            var mustBeSavedRule = _policyRetrievalRules.GetMustBeSavedRule();
            if (!mustBeSavedRule.IsSatisfiedBy(policy))
            {
                return PolicyRetrievalStatus.NotSaved;
            }

            //Check credentials
            if (_customerAuthenticationService.Authenticate(quoteReference, password).Status !=
                CustomerResultStatus.Success)
            {
                return PolicyRetrievalStatus.InvalidPassword;
            }

            //Not submitted
            var notSubmittedRule = _policyRetrievalRules.GetNotSubmittedRule();
            if (!notSubmittedRule.IsSatisfiedBy(policy))
            {
                return PolicyRetrievalStatus.AlreadySubmitted;
            }

            //Not referred to underwriter
            var notReferredRule = _policyRetrievalRules.GetNotReferredToUnderwriterRule();
            if (!notReferredRule.IsSatisfiedBy(policy))
            {
                return PolicyRetrievalStatus.ReferredToUnderwriter;
            }

            //not locked out
            var notLockedOutRule = _policyRetrievalRules.GetNotLockedOutRule();
            if (!notLockedOutRule.IsSatisfiedBy(policy))
            {
                return PolicyRetrievalStatus.LockedOutDueToRefer;
            }

            return PolicyRetrievalStatus.CanBeRetrieved;
        }
    }
}
