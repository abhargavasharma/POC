using System;
using TAL.QuoteAndApply.DataModel.Personal;

namespace TAL.QuoteAndApply.Policy.Models
{
    public interface IRatingFactors
    {
        Gender Gender { get; }
        DateTime DateOfBirth { get; }
        ResidencyStatus Residency { get; }
        SmokerStatus SmokerStatus { get; }
        long AnnualIncome { get; }
    }
}