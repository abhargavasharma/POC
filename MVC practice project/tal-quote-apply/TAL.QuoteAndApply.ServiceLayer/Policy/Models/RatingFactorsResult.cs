using System;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class RatingFactorsResult
    {
        public char? Gender { get; private set; }
        public DateTime? DateOfBirth { get; private set; }
        public bool? IsAustralianResident { get; private set; }
        public string SmokerStatus { get; private set; }
        public string OccupationCode { get; private set; }
        public string OccupationTitle { get; private set; }
        public string IndustryCode { get; private set; }
        public string IndustryTitle { get; private set; }
        public long Income { get; private set; }

        public RatingFactorsResult(char? gender, DateTime? dateOfBirth, bool? isAustralianResident, string smokerStatus,
            string occupationCode, string occupationTitle, long income, string industryCode, string industryTitle)
        {
            Gender = gender;
            DateOfBirth = dateOfBirth;
            IsAustralianResident = isAustralianResident;
            SmokerStatus = smokerStatus;
            OccupationCode = occupationCode;
            OccupationTitle = occupationTitle;
            Income = income;
            IndustryCode = industryCode;
            IndustryTitle = industryTitle;
        }

        public bool IsRatingFactorsValidForInforce { get; set; }
    }
}