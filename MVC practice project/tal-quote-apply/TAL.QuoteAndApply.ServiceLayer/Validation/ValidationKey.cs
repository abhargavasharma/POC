namespace TAL.QuoteAndApply.ServiceLayer.Validation
{
    public enum ValidationKey
    {
        MinimumAge,
        MaximumAge,
        AustralianResidency,
        EligiblePremiumType,
        AnnualIncome,
        MaximumCoverAmount,
        MinimumCoverAmount,
        MaxGreaterThanMinCoverAmount,
        MinimumSportsCover,
        MinimumPremiumRelief,
        InvalidPlanSelection,
        InvalidCoverSelection,
        InvalidRiderSelection,
        InvalidRiderCoverSelection,
        InvalidOptionSelection,
        AnnualIncomeMissingForCoverAmount,
        SelectedCoverUnderwritingDeclined,
        OptionMaximumAge,
        LinkedToCpiRequired,
        PremiumHolidayRequired,
        PremiumReliefOption,
        InvalidAdobeLeadData,
		RequiredPlanVariable,
        PlanVariableInvalid,
        RequiredPlanOption,
        RequiredPlanCoverAmount,
        Submission
    }

    public enum PaymentValidationKey
    {
        DirectDebitBsbNumber,
        DirectDebitAccountNumber,
        DirectDebitAccountName
    }
}