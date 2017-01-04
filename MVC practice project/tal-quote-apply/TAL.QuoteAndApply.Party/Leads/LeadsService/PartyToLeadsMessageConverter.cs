using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Party.Configuration;
using TAL.QuoteAndApply.Party.Models;

namespace TAL.QuoteAndApply.Party.Leads.LeadsService
{
    public interface IPartyToLeadsMessageConverter
    {
        MarketingInquiryProcess From(IParty party, PolicySource policySource);
    }

    public class PartyToLeadsMessageConverter : IPartyToLeadsMessageConverter
    {
        private readonly ILeadConfigurationProvider _leadConfigurationProvider;

        public PartyToLeadsMessageConverter(ILeadConfigurationProvider leadConfigurationProvider)
        {
            _leadConfigurationProvider = leadConfigurationProvider;
        }

        private const string CREATE_LEADS = "CreateLeads";
        private const string UPDATE_LEADS = "UpdateLeads";

        public MarketingInquiryProcess From(IParty party, PolicySource policySource)
        {
            var createLeadsMsg = new MarketingInquiryProcess
            {
                MessageHeader = LeadsServiceMessageHeaderBuilder.GetMessageHeader(),
                MessageKeys = GetTransactionFunctionCode(party),
                MarketingInquiry = new MarketingInquiryProcessMarketingInquiry
                {
                    Lead = new []
                    {
                        new Lead_Type
                        {
                            Address = GetAddress(party),
                            Person = GetPerson(party),
                            ExternalIdentifier = GetExternalIdentifiers(party),
                            RemarkInformation = GetRemarkInformation(policySource)
                        }
                    }
                }
            };

            return createLeadsMsg;
        }

        private RemarkInformation_Type[] GetRemarkInformation(PolicySource policySource)
        {
            return new[]
            {
                new RemarkInformation_Type
                {
                    RemarkTypeCode = new RemarkTypeCode_Type {Value = new XmlQualifiedName("ProspectNotes")},
                    Remark = $"JTWorkaroundWithCN-{policySource}" 
                }
            };
        }

        private static MarketingInquiryProcessMessageKeys GetTransactionFunctionCode(IParty party)
        {
            var function = CREATE_LEADS;

            if (party.LeadId.HasValue)
            {
                function = UPDATE_LEADS;
            }

            return new MarketingInquiryProcessMessageKeys
            {
                TransactionFunctionCode = function
            };
        }

        private ExternalIdentifier_Type[] GetExternalIdentifiers(IParty party)
        {
            var returnObj = new List<ExternalIdentifier_Type>();

            returnObj.Add(new ExternalIdentifier_Type
            {
                Id = _leadConfigurationProvider.BrandCode,
                TypeCode = new ExternalIdentifierTypeCode_Type
                {
                    Value = new XmlQualifiedName("BrandCode")
                }
            });

            if (party.LeadId.HasValue)
            {
                returnObj.Add(new ExternalIdentifier_Type
                {
                    Id = party.LeadId.Value.ToString(),
                    TypeCode = new ExternalIdentifierTypeCode_Type
                    {
                        Value = new XmlQualifiedName("ADOBEId")
                    }
                });

            }

            return returnObj.ToArray();
        }

        private Person_Type GetPerson(IParty party)
        {
            return new Person_Type
            {
                key="1",
                ExternalIdentifier = new []
                {
                    new PersonExternalIdentifier_Type
                    {
                        TypeCode = new PersonExternalIdentifierTypeCode_Type { Value = new XmlQualifiedName("BrandProspectId") }
                    }
                },
                PersonName = GetPersonName(party),
                PersonCommunication = GetPersonCommunication(party),
                GenderCode = GetGender(party.Gender),
                BirthDate = party.DateOfBirth,
                BirthDateSpecified = true
            };
        }

        private PersonName_Type[] GetPersonName(IParty party)
        {
            return new []
            {
                new PersonName_Type
                {
                    TypeCode = new PersonNameTypeCode_Type { Value = new XmlQualifiedName("Primary")},
                    Surname = StringOrNull(party.Surname),
                    GivenName = StringOrNull(party.FirstName),
                    PersonTitlePrefix = GetTitle(party.Title)
                }
            };
        }

        private string GetTitle(Title title)
        {
            if (title == Title.Unknown)
                return null;

            return title.ToString();
        }

        private string StringOrNull(string val)
        {
            if (string.IsNullOrWhiteSpace(val))
                return null;

            return val;
        }

        private PersonCommunication_Type GetPersonCommunication(IParty party)
        {
            return new PersonCommunication_Type
            {
                Telephone = GetPhoneTypes(party),
                Email = GetEmail(party),
                MailingAddress = GetMailingAddress(party)
            };
        }

        private MailingAddress_Type[] GetMailingAddress(IParty party)
        {
            if (HasAnyAddressFields(party))
            {
                return new[]
                {
                    new MailingAddress_Type
                    {
                        PrimaryIndicator = true,
                        PrimaryIndicatorSpecified = true,
                        AddressReferences = new AddressReferences_Type {addressReference = "1"}
                    }
                };
            }

            return null;
        }

        private Email_Type[] GetEmail(IParty party)
        {
            if (!string.IsNullOrWhiteSpace(party.EmailAddress))
            {
                return new[]
                {
                    new Email_Type
                    {
                        EmailAddress = new[] {party.EmailAddress}
                    }
                };
            }

            return null;
        }

        private Telephone_Type[] GetPhoneTypes(IParty party)
        {
            var telephones = new List<Telephone_Type>();

            if (!string.IsNullOrWhiteSpace(party.HomeNumber))
            {
                telephones.Add(new Telephone_Type
                {
                    TypeCode = new TelephoneTypeCode_Type { name = "Wired", Value = new XmlQualifiedName("Wired") },
                    PhoneNumberUnformatted = party.HomeNumber
                });
            }
            if (!string.IsNullOrWhiteSpace(party.MobileNumber))
            {
                telephones.Add(new Telephone_Type
                {
                    TypeCode = new TelephoneTypeCode_Type { name = "Wireless", Value = new XmlQualifiedName("Wireless") },
                    PhoneNumberUnformatted = party.MobileNumber
                });
            }

            if (telephones.Any())
                return telephones.ToArray();

            return null;
        }

        private GenderCode_Type GetGender(Gender gender)
        {
            if (gender == Gender.Unknown)
            {
                return null;
            }

            var genderCode = "M";

            if(gender == Gender.Female)
                genderCode = "F";

            return new GenderCode_Type {name = gender.ToString(), Value = new XmlQualifiedName(genderCode)};
        }

        private Address_Type GetAddress(IParty party)
        {
            if (HasAnyAddressFields(party))
            {
                return new Address_Type
                {
                    key = "1",
                    PostalCode = GetPostCode(party),
                    StateOrProvinceCode = GetState(party),
                    SuburbName = StringOrNull(party.Suburb),
                    LineOne = StringOrNull(party.Address),
                    CountryCode = new Code_Type() { Value = "AUS" }
                };
            }

            return null;

        }

        private bool HasAnyAddressFields(IParty party)
        {
            return !string.IsNullOrWhiteSpace(party.Address) ||
                   !string.IsNullOrWhiteSpace(party.Postcode) ||
                   !string.IsNullOrWhiteSpace(party.Suburb) ||
                   party.State != State.Unknown;
        }

        private Code_Type GetPostCode(IParty party)
        {
            if (!string.IsNullOrWhiteSpace(party.Postcode))
            {
                return new Code_Type {Value = party.Postcode};
            }

            return null;
        }

        private Code_Type GetState(IParty party)
        {
            if (party.State == State.Unknown)
            {
                return null;
            }

            return new Code_Type { Value = party.State.ToString() };
        }
    }
}
