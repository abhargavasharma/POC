using System.Collections.Generic;

namespace TAL.QuoteAndApply.Product.Models.Definition
{
    public enum Should
    {
        BeAllSelected,
        BeSomeSelected,
        NotAnyBeSelected 
    }

    public enum From
    {
        Product,
        CurrentPlan,
        Rider
    }

    public class ProductConfigRule
    {
        public ProductConfigRule() : this(Should.BeAllSelected, From.CurrentPlan)
        {}

        public ProductConfigRule(Should requirement, From context, params string[] features)
        {
            Requirement = requirement;
            Scope = context;
            Features = features;
        }

        public Should Requirement { get; }
        public From Scope { get; }
        public IEnumerable<string> Features { get; }

        public int? MinimumEntryAgeNextBirthday { get; private set; }
        public int? MaximumEntryAgeNextBirthday { get; private set; }
        public string[] SupportedOccupationClasses { get; private set; }
        public string[] UnsupportedOccupationClasses { get; private set; }
        public int[] SupportedWaitingPeriods { get; private set; }
        public bool LinkedToCpiRequired { get; private set; }

        public ProductConfigRule WithMinimumEntryAgeNextBirthday(int minimumEntryAgeNextBirthday)
        {
            MinimumEntryAgeNextBirthday = minimumEntryAgeNextBirthday;
            return this;
        }

        public ProductConfigRule WithMaximumEntryAgeNextBirthday(int maximumEntryAgeNextBirthday)
        {
            MaximumEntryAgeNextBirthday = maximumEntryAgeNextBirthday;
            return this;
        }

        public ProductConfigRule WithSupportedOccupationClasses(string[] supportedOccupationClasses)
        {
            SupportedOccupationClasses = supportedOccupationClasses;
            return this;
        }

        public ProductConfigRule DoNotAllowOccupationClasses(params string[] unsupportedOccupationClasses)
        {
            UnsupportedOccupationClasses = unsupportedOccupationClasses;
            return this;
        }

        public ProductConfigRule WithSupportedWaitingPeriods(int[] supportedWaitingPeriods)
        {
            SupportedWaitingPeriods = supportedWaitingPeriods;
            return this;
        }

        public ProductConfigRule WithInflationProtectionRequired()
        {
            LinkedToCpiRequired = true;
            return this;
        }
    }
}