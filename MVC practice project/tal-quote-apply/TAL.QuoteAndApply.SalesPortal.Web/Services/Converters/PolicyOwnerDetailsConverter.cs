using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface IPolicyOwnerDetailsConverter
    {
        PolicyOwnerDetailsRequest From(PolicyOwnerDetailsParam param);
        PolicyOwnerDetailsParam From(PolicyOwnerDetailsRequest request);
    }

    public class PolicyOwnerDetailsConverter: IPolicyOwnerDetailsConverter
    {
        public PolicyOwnerDetailsRequest From(PolicyOwnerDetailsParam param)
        {
            var request = new PolicyOwnerDetailsRequest
            {
                FundName = param.FundName,

                Title = param.Title,
                FirstName = param.FirstName,
                Surname = param.Surname,

                ExternalCustomerReference = param.ExternalCustomerReference,

                Address = param.Address,
                Suburb = param.Suburb,
                Postcode = param.Postcode,
                State = param.State,

                MobileNumber = param.MobileNumber,
                HomeNumber = param.HomeNumber,
                EmailAddress = param.EmailAddress,
                IsCompleted = param.IsCompleted
            };

            if (param.PartyConsentsParam != null)
            {
                request.PartyConsents = param.PartyConsentsParam.Consents;
                request.ExpressConsent = param.PartyConsentsParam.ExpressConsent;
            }

            return request;
        }

        public PolicyOwnerDetailsParam From(PolicyOwnerDetailsRequest request)
        {
            return new PolicyOwnerDetailsParam
            {
                FundName = request.FundName,

                Title = request.Title,
                FirstName = request.FirstName,
                Surname = request.Surname,

                ExternalCustomerReference = request.ExternalCustomerReference,

                Address = request.Address,
                Suburb = request.Suburb,
                Postcode = request.Postcode,
                State = request.State,

                MobileNumber = request.MobileNumber,
                HomeNumber = request.HomeNumber,
                EmailAddress = request.EmailAddress,

                PartyConsentsParam = new PartyConsentParam(request.PartyConsents, request.ExpressConsent)
            };
        }
    }
}