using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Rules;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class RiskBeneficiaryRuleResult
    {
        public RiskBeneficiaryRuleResult(string key, bool isSatisfied, string[] messages)
        {
            Key = key;
            IsSatisfied = isSatisfied;
            Messages = messages;
        }

        public string Key { get; private set; }
        public bool IsSatisfied { get; private set; }
        public string[] Messages { get; private set; }
    }

    public class RiskBeneficiaryValidationModel
    {
        public RiskBeneficiaryValidationModel(int beneficiaryId)
        {
            rules = new List<RiskBeneficiaryRuleResult>();
            BeneficiaryId = beneficiaryId; 
        }

        private readonly List<RiskBeneficiaryRuleResult> rules;

        public IReadOnlyList<RiskBeneficiaryRuleResult> ValidationErrors
        {
            get { return rules; }
        }

        public int BeneficiaryId { get; private set; }

        public bool IsValid
        {
            get { return !ValidationErrors.Any(); }
        }

        public void AddError(RuleResult brokeRule)
        {
            rules.Add(new RiskBeneficiaryRuleResult(brokeRule.Key, brokeRule.IsSatisfied, brokeRule.Messages));
        }

        public void AddError(RuleResult brokeRule, string message)
        {
            rules.Add(new RiskBeneficiaryRuleResult(brokeRule.Key, brokeRule.IsSatisfied, new [] { message }));
        }
    }

    public class RiskBeneficiaryDetailsParam
    {
        public string Address { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public int Share { get; set; }
        public int? BeneficiaryRelationshipId { get; set; }


        public string Title { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }

        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }

        public int RiskId { get; set; }
        public int Id { get; set; }
    }
}
