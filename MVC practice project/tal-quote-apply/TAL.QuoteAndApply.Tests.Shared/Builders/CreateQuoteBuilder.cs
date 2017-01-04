using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.ServiceLayer.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.Tests.Shared.Helpers;
using PolicySource = TAL.QuoteAndApply.ServiceLayer.Models.PolicySource;

namespace TAL.QuoteAndApply.Tests.Shared.Builders
{
    public class CreateQuoteBuilder
    {
        private bool _validateResidency;
        private PolicySource _policySource;
        private string _brand;

        //Rating Factors
        private char _gender;
        private DateTime _dateOfBirth;
        private bool _australianResident;
        private bool _isSmoker;
        private string _occupationCode;
        private string _industryCode;
        private int _annualIncome;

        //Personal Information
        private string _title;
        private string _firstname;
        private string _surname;
        private string _mobileNumber;
        private string _homeNumber;
        private string _state;
        private string _emailAddress;
        private string _suburb;
        private string _postcode;
        private string _address;
        private string _customerReference;

        public CreateQuoteBuilder()
        {
            WithDefaults();
        }

        private void WithDefaults()
        {
            _gender = 'M';
            _dateOfBirth = DateTime.Today.AddDays(1).AddYears(-(19));
            _australianResident = true;
            _occupationCode = UnderwritingHelper.OccupationCode_AccountExecutive;
            _industryCode = UnderwritingHelper.IndustryCode_AdvertisingAndMarketing;
            _annualIncome = 100000;

            _title = "Mr";
            _firstname = "Art";
            _surname = "Test";
            _mobileNumber = "0400000000";
            _homeNumber = "0800000000";
            _state = "VIC";
            _emailAddress = "avandalay@vandelayindustries.com.au";
            _postcode = "1234";
            _suburb = "Kensington";
            _address = "12 Happy St";
            _brand = "TAL";

            _validateResidency = false;
            _policySource = PolicySource.Unknown;
        }

        public CreateQuoteBuilder WithJourneyType(PolicySource policySource)
        {
            _policySource = policySource;
            return this;
        }

        public CreateQuoteBuilder WithNoOccupation()
        {
            _occupationCode = "";
            _industryCode = "";
            return this;
        }

        public CreateQuoteBuilder AsCommercialPilot()
        {
            _occupationCode = UnderwritingHelper.OccupationCode_CommercialAirlinePilot;
            _industryCode = UnderwritingHelper.IndustryCode_Aviation;
            return this;
        }

        public CreateQuoteBuilder AsComputerAnalyst()
        {
            _occupationCode = UnderwritingHelper.OccupationCode_ComputerAnalyst;
            _industryCode = UnderwritingHelper.IndustryCode_InformationTechnology;
            return this;
        }

        public CreateQuoteBuilder AsAge(int age)
        {
            _dateOfBirth = DateTime.Now.AddYears(-age).Date;
            return this;
        }

        public CreateQuoteParam Build()
        {
            var ratingFactors = new RatingFactorsParam(
                _gender, _dateOfBirth, 
                _australianResident, new SmokerStatusHelper(_isSmoker), 
                _occupationCode, _industryCode, _annualIncome);

            var personalInformation = new PersonalInformationParam(
                _title, _firstname, _surname,
                _mobileNumber, _homeNumber, _state, _emailAddress, null, _suburb, _address, _postcode, _customerReference);

            return new CreateQuoteParam(ratingFactors, personalInformation, _validateResidency, _policySource, _brand);
        }
    }
}
