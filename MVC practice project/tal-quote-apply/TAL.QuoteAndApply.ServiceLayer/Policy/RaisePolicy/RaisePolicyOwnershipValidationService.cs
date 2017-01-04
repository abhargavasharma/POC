using System.Linq;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Party.Services;
using TAL.QuoteAndApply.Policy.Service;
using TAL.QuoteAndApply.ServiceLayer.Search.Clients;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy
{
    public interface IRaisePolicyOwnershipValidationService
    {
        bool IsCompleted(string quoteReferenceNumber);
    }
    public class RaisePolicyOwnershipValidationService : IRaisePolicyOwnershipValidationService
    {
        private readonly IPolicyOwnerService _policyOwnerService;
        private readonly IRaisePolicyValidationService _raisePolicyValidationService;
        private readonly IRaisePolicyFactory _raisePolicyFactory;
        private readonly IPolicyOverviewProvider _policyOverviewProvider;
        private readonly IPartyService _partyService;
        private readonly IExternalRefDetailsFactory _externalCustomerRefConfigDetails;

        public RaisePolicyOwnershipValidationService(IPolicyOwnerService policyOwnerService, 
            IRaisePolicyValidationService raisePolicyValidationService,
            IRaisePolicyFactory raisePolicyFactory,
            IPolicyOverviewProvider policyOverviewProvider, IPartyService partyService,
            IExternalRefDetailsFactory externalCustomerRefConfigDetails)
        {
            _policyOwnerService = policyOwnerService;
            _raisePolicyValidationService = raisePolicyValidationService;
            _raisePolicyFactory = raisePolicyFactory;
            _policyOverviewProvider = policyOverviewProvider;
            _partyService = partyService;
            _externalCustomerRefConfigDetails = externalCustomerRefConfigDetails;
        }
        public bool IsCompleted(string quoteReferenceNumber)
        {
            var uberPolicyObject = _raisePolicyFactory.GetFromQuoteReference(quoteReferenceNumber);
            
            if (_raisePolicyValidationService.ValidateOwnerForInforce(uberPolicyObject.Owner).Any(r => !r.IsSatisfied))
            {
                return false;
            }

            if (!string.IsNullOrEmpty(uberPolicyObject.BrandKey))
            {
                var externalCustomerRefSettings = _externalCustomerRefConfigDetails.ExternalCustomerRefConfigDetails(uberPolicyObject.BrandKey);
                if (externalCustomerRefSettings.ExternalCustomerRefRequired == ExternalCustomerRefRequired.Mandatory)
                {
                    var validateOwnerExternalCustomerRefForInforce = _raisePolicyValidationService
                        .ValidateOwnerExternalCustomerRefForInforce(uberPolicyObject.Owner.ExternalCustomerReference)
                        .Where(r => !r.IsSatisfied);
                    if (validateOwnerExternalCustomerRefForInforce.Any())
                    {
                        return false;
                    }
                }
            }

            var ownerShipType = _policyOwnerService.GetOwnerShipType(quoteReferenceNumber);
            if (ownerShipType == PolicyOwnerType.SelfManagedSuperFund || ownerShipType == PolicyOwnerType.SuperannuationFund)
            {
                var ownerdetails = _policyOverviewProvider.GetOwnerDetailsFor(quoteReferenceNumber);
                if (string.IsNullOrEmpty(ownerdetails.FundName))
                {
                    return false;
                }

                var risk = uberPolicyObject.Risks[0];
                var partyDto = _partyService.GetParty(risk.PartyId);
                if (_raisePolicyValidationService.ValidatePartyForInforce(partyDto).Any(r => !r.IsSatisfied))
                {
                    return false;
                }
            }
           
            return true;
        }
    }
}
