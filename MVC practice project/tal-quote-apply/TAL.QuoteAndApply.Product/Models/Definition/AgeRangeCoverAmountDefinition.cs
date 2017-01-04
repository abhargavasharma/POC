using System;

namespace TAL.QuoteAndApply.Product.Models.Definition
{
    public class AgeRangeCoverAmountDefinition
    {
        public AgeRangeDefinition AgeRangeDefinition { get; private set; }

        public int? AnnualIncomeFactor { get; private set; }
        public int MaxCoverAmount { get; private set; }

        public int? NoIncomeMaxCover { get; private set; }

        public AgeRangeCoverAmountDefinition(AgeRangeDefinition ageRangeDefinition, int? annualIncomeFactor, int maxCoverAmount, int? unrestrictedMaxAmount)
        {
            AgeRangeDefinition = ageRangeDefinition;
            AnnualIncomeFactor = annualIncomeFactor;
            MaxCoverAmount = maxCoverAmount;
            NoIncomeMaxCover = unrestrictedMaxAmount;
        }

        private AgeRangeCoverAmountDefinition()
        {
        }

        public static AgeRangeCoverAmountDefinitionBuilder Builder()
        {
            return new AgeRangeCoverAmountDefinitionBuilder();
        }


        public class AgeRangeCoverAmountDefinitionBuilder
        {
            private readonly AgeRangeCoverAmountDefinition _ageRangeCoverAmountDefinition = new AgeRangeCoverAmountDefinition();
            
            public AgeRangeCoverAmountDefinition Build()
            {
                if (_ageRangeCoverAmountDefinition.AgeRangeDefinition == null)
                {
                    throw new InvalidOperationException("AgeRangeDefinition is required");
                }

                if (_ageRangeCoverAmountDefinition.MaxCoverAmount == default(int))
                {
                    throw new InvalidOperationException("MaxCoverAmount is required");
                }

                return _ageRangeCoverAmountDefinition;
            }

            public AgeRangeCoverAmountDefinitionBuilder WithAgeRangeDefinition(int lowerAge, int upperAge)
            {
                _ageRangeCoverAmountDefinition.AgeRangeDefinition = new AgeRangeDefinition(lowerAge, upperAge);

                return this;
            }

            public AgeRangeCoverAmountDefinitionBuilder WithMaxCover(int maxCover)
            {
                _ageRangeCoverAmountDefinition.MaxCoverAmount = maxCover;

                return this;
            }

            public AgeRangeCoverAmountDefinitionBuilder WithNoIncomeMaxCoverAmount(int noIncomeMaxCoverAmount)
            {
                _ageRangeCoverAmountDefinition.NoIncomeMaxCover = noIncomeMaxCoverAmount;

                return this;
            }

            public AgeRangeCoverAmountDefinitionBuilder WithAnnualIncomeFactor(int factor)
            {
                _ageRangeCoverAmountDefinition.AnnualIncomeFactor = factor;

                return this;
            }
        }
    }
}
