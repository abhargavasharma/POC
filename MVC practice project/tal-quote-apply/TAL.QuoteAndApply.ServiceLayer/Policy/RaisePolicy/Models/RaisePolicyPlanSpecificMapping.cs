using System;
using System.Collections.Generic;
using TAL.QuoteAndApply.Product.Definition;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models
{
    public class RaisePolicyPlanSpecificMapping
    {
        public IDictionary<string, RaisePolicyCoverSpecificMapping> Covers { get; }
        public string Name { get; }
        public string Code { get; }

        public bool IsRider
        {
            get
            {
                return Code == ProductRiderConstants.CriticalIllnessRiderCode ||
                       Code == ProductRiderConstants.PermanentDisabilityRiderCode;
            }
        }

        public bool IsIncomeProtection
        {
            get { return Code == ProductPlanConstants.IncomeProtectionPlanCode; }
        }

        public bool IsTpd
        {
            get
            {
                return Code == ProductPlanConstants.PermanentDisabilityPlanCode ||
                       Code == ProductRiderConstants.PermanentDisabilityRiderCode;
            }
        }

        public bool IsCriticalIllness
        {
            get
            {
                return Code == ProductPlanConstants.CriticalIllnessPlanCode ||
                       Code == ProductRiderConstants.CriticalIllnessRiderCode;
            }
        }

        public RaisePolicyPlanSpecificMapping(string code, string name, IDictionary<string, RaisePolicyCoverSpecificMapping> covers)
        {
            Covers = covers;
            Name = name;
            Code = code;
        }
    }

    public enum RaisePolicyCoverType
    {
        AccidentOrSeriousInjury,
        Illness,
        SportsOrCancer
    }

    public class RaisePolicyCoverSpecificMapping
    {
        public RaisePolicyCoverType CoverType { get; }

        public string CoverBlockCode
        {
            get
            {
                switch (CoverType)
                {
                    case RaisePolicyCoverType.AccidentOrSeriousInjury:
                        return "AC";
                    case RaisePolicyCoverType.Illness:
                        return "IL";
                    case RaisePolicyCoverType.SportsOrCancer:
                        return "SP";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public RaisePolicyCoverSpecificMapping(RaisePolicyCoverType coverType)
        {
            CoverType = coverType;
        }
    }
}