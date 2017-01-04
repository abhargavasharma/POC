using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Infrastructure.Extensions;

namespace TAL.QuoteAndApply.Party.Leads.LeadsService
{
    public interface IAdobeServiceResultConverter
    {
        GetLeadResult From(MarketingInquiryProcessResult marketingInquiryProcessResult);
        GetPreferredCommunicationResult From(PartyCommunicationInquiryNotify communicationPreferences);
    }
    class AdobeServiceResultConverter : IAdobeServiceResultConverter
    {
        private const string Wired = "Wired";
        private const string Wireless = "Wireless";
        private const string AdobeId = "ADOBEId";

        public GetLeadResult From(MarketingInquiryProcessResult marketingInquiryProcessResult)
        {
            if (marketingInquiryProcessResult?.MarketingInquiry?.Lead?.First() != null)
            {
                var firstLead = marketingInquiryProcessResult.MarketingInquiry.Lead.First();
                var concatenatedAddress = string.Concat(firstLead.Address.LineOne,
                    firstLead.Address.LineTwo.IsNullOrWhiteSpace() ? "" : $", {firstLead.Address.LineTwo}",
                    firstLead.Address.LineThree.IsNullOrWhiteSpace() ? "" : $", {firstLead.Address.LineThree}");
                var returnObj = new GetLeadResult()
                {
                    AdobeId =
                        firstLead
                            .ExternalIdentifier.First(x => x.TypeCode.Value.Name.ToString() == AdobeId)
                            .Id,
                    DateOfBirth = firstLead.Person.BirthDate.Date,
                    EmailAddress =
                        firstLead
                            .Person.PersonCommunication.Email.First()
                            .EmailAddress.First(),
                    Title = firstLead.Person.PersonName.First().PersonTitlePrefix,
                    FirstName = firstLead.Person.PersonName.First().GivenName,
                    Surname = firstLead.Person.PersonName.First().Surname,
                    HomeNumber =
                        firstLead
                            .Person.PersonCommunication.Telephone.First(t => t.TypeCode.Value.Name == Wired)
                            .PhoneNumberUnformatted,
                    MobileNumber =
                        firstLead
                            .Person.PersonCommunication.Telephone.First(t => t.TypeCode.Value.Name == Wireless)
                            .PhoneNumberUnformatted,
                    State =
                        firstLead.Address.StateOrProvinceCode.Value,
                    Gender = SetGender(firstLead.Person.GenderCode.name),
                    Postcode = firstLead.Address.PostalCode.Value,
                    Suburb = firstLead.Address.SuburbName,
                    Address = concatenatedAddress
                };
                return returnObj;
            }
            return null;
        }

        public GetPreferredCommunicationResult From(PartyCommunicationInquiryNotify communicationPreferences)
        {
            var getPreferredCommunicationResult = new GetPreferredCommunicationResult()
            {
                ExpressConsent =
                    communicationPreferences.PartyCommunication.First()
                        .CommunicationPreferences.First()
                        .ExpressConsent.ExpressConsentIndicator,
                ExpressConsentUpdatedTs =
                    communicationPreferences.PartyCommunication.First()
                        .CommunicationPreferences.First()
                        .ExpressConsent.ExpressConsentStartDate,
                DncMobile =
                    communicationPreferences.PartyCommunication.First()
                        .Person.First()
                        .PersonCommunication.Telephone.First(t => t.TypeCode.name == Wireless)
                        .CommunicationPreferences.First()
                        .DoNotContactIndicator,
                DncHomeNumber =
                    communicationPreferences.PartyCommunication.First()
                        .Person.First()
                        .PersonCommunication.Telephone.First(t => t.TypeCode.name == Wired)
                        .CommunicationPreferences.First()
                        .DoNotContactIndicator,
                DncEmail =
                    communicationPreferences.PartyCommunication.First()
                        .Person.First()
                        .PersonCommunication.Email.First()
                        .CommunicationPreferences.First()
                        .DoNotContactIndicator,
                DncPostalMail =
                    communicationPreferences.PartyCommunication.First()
                        .Person.First()
                        .PersonCommunication.MailingAddress.First()
                        .CommunicationPreferences.First()
                        .DoNotContactIndicator
            };
            return getPreferredCommunicationResult;
        }

        private string SetGender(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return name.ToCharArray()[0].ToString();
            }
            return "";
        }
    }
}
