using System;
using System.Collections.Generic;
using System.Xml;
using TAL.QuoteAndApply.Party.Configuration;
using TAL.QuoteAndApply.Party.Models;

namespace TAL.QuoteAndApply.Party.Leads.LeadsService
{
    public interface IPartyCommunicationMessageConverter
    {
        PartyCommunicationProcess From(IParty party, IPartyConsent partyConsent);
    }

    public class PartyCommunicationMessageConverter : IPartyCommunicationMessageConverter
    {
        private readonly ILeadConfigurationProvider _leadConfigurationProvider;

        public PartyCommunicationMessageConverter(ILeadConfigurationProvider leadConfigurationProvider)
        {
            _leadConfigurationProvider = leadConfigurationProvider;
        }

        public PartyCommunicationProcess From(IParty party, IPartyConsent partyConsent)
        {
            var partyCommunicationMessage = new PartyCommunicationProcess
            {
                MessageHeader = LeadsServiceMessageHeaderBuilder.GetMessageHeader(),
                MessageKeys = GetMessageKeys(),
                PartyCommunication = GetPartyCommunication(party, partyConsent)
            };

            return partyCommunicationMessage;
        }

        private PartyCommunication[] GetPartyCommunication(IParty party, IPartyConsent partyConsent)
        {
            return new[]
            {
                new PartyCommunication
                {
                    Person = GetPerson(party, partyConsent),
                    CommunicationPreferences = GetCommunicationPreferences(partyConsent.ExpressConsent, partyConsent.ExpressConsentUpdatedTs),
                    ExternalIdentifier = GetExternalIdentifiers(party)
                }
            };
        }

        private Person_Type[] GetPerson(IParty party, IPartyConsent partyConsent)
        {
            return new[]
            {
                new Person_Type
                {
                    key = "1",
                    PersonCommunication = new PersonCommunication_Type
                    {
                        Telephone = GetTelephones(partyConsent),
                        Email = GetEmail(partyConsent.DncEmail),
                        MailingAddress = GetMailingAddress(party, partyConsent.DncPostalMail)
                    },
                    BusinessCommunication = new BusinessCommunication_Type
                    {
                        Telephone =  new[]
                        {
                            new Telephone_Type
                            {
                                TypeCode = new TelephoneTypeCode_Type
                                {
                                    name = "Wired",
                                    Value = new XmlQualifiedName("Wired")
                                },
                                CommunicationPreferences = new[]
                                {
                                    new CommunicationPreferences_Type
                                    {
                                        DoNotContactIndicator = false,
                                        DoNotContactIndicatorSpecified = true
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        private MailingAddress_Type[] GetMailingAddress(IParty party, bool dnc)
        {
            //todo do we care if address sepcified or should we always send?

            return new[]
            {
                new MailingAddress_Type
                {
                    CommunicationPreferences = new []
                    {
                        new CommunicationPreferences_Type
                        {
                            DoNotContactIndicator = dnc,
                            DoNotContactIndicatorSpecified = true
                        }
                    }
                }
            };
        }

        private Email_Type[] GetEmail(bool dnc)
        {
            //todo do we care if address sepcified or should we always send?

            return new []
            {
                new Email_Type
                {
                    CommunicationPreferences = new []
                    {
                        new CommunicationPreferences_Type
                        {
                            DoNotContactIndicator = dnc,
                            DoNotContactIndicatorSpecified = true
                        }
                    }
                }
            };
        }

        private Telephone_Type[] GetTelephones(IPartyConsent partyConsent)
        {
            //todo do we care if address sepcified or should we always send?

            return new[]
            {
                new Telephone_Type
                {
                    TypeCode = new TelephoneTypeCode_Type
                    {
                        name = "Wired",
                        Value = new XmlQualifiedName("Wired")
                    },
                    CommunicationPreferences = new[]
                    {
                        new CommunicationPreferences_Type
                        {
                            DoNotContactIndicator = partyConsent.DncHomeNumber,
                            DoNotContactIndicatorSpecified = true
                        }
                    }
                },
                new Telephone_Type
                {
                    TypeCode = new TelephoneTypeCode_Type
                    {
                        name = "Wireless",
                        Value = new XmlQualifiedName("Wireless")
                    },
                    CommunicationPreferences = new[]
                    {
                        new CommunicationPreferences_Type
                        {
                            DoNotContactIndicator = partyConsent.DncMobile,
                            DoNotContactIndicatorSpecified = true
                        }
                    }
                }
            };
        }

        private CommunicationPreferences_Type[] GetCommunicationPreferences(bool expressConsent, DateTime? expressConsentDate)
        {
            return new[]
            {
                new CommunicationPreferences_Type
                {
                    ExpressConsent = GetExpressConsent(expressConsent, expressConsentDate)
                }
            };
        }

        private ExpressConsent_Type GetExpressConsent(bool expressConsent, DateTime? expressConsentDate)
        {
            var expressConsentType = new ExpressConsent_Type();
            expressConsentType.ExpressConsentIndicator = expressConsent;
            expressConsentType.ExpressConsentIndicatorSpecified = true;

            if (expressConsent && expressConsentDate.HasValue)
            {
                expressConsentType.ExpressConsentStartDate = expressConsentDate.Value;
                expressConsentType.ExpressConsentStartDateSpecified = true;
                expressConsentType.ExpressConsentEndDate = expressConsentDate.Value.AddYears(50);
                expressConsentType.ExpressConsentEndDateSpecified = true;
            }

            return expressConsentType;
        }

        private ExternalIdentifier_Type[] GetExternalIdentifiers(IParty party)
        {
            if (!party.LeadId.HasValue)
                return null;

            return new[]
            {
                new ExternalIdentifier_Type
                {
                    TypeCode = new ExternalIdentifierTypeCode_Type { Value = new XmlQualifiedName("ADOBEId") },
                    Id = party.LeadId.Value.ToString()
                },
                new ExternalIdentifier_Type
                {
                    TypeCode = new ExternalIdentifierTypeCode_Type { Value = new XmlQualifiedName("BrandCode") },
                    Id = _leadConfigurationProvider.BrandCode
                }
            };
        }

        private PartyCommunicationProcessMessageKeys GetMessageKeys()
        {
            return new PartyCommunicationProcessMessageKeys
            {
                TransactionFunctionCode = "UpdateCommunicationPreferences"
            };
        }
    }
}