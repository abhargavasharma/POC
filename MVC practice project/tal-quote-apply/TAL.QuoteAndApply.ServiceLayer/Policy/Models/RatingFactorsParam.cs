using System;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class RatingFactorsParam
    {
        public char Gender { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public bool? IsAustralianResident { get; private set; }
        public SmokerStatusHelper SmokerStatus { get; private set; }
        public string OccupationCode { get; private set; }
        public string IndustryCode { get; private set; }
        public long Income { get; private set; }

        public RatingFactorsParam(char gender, DateTime dateOfBirth, bool? isAustralianResident, SmokerStatusHelper smokerStatus,
            string occupationCode, string industryCode, long income)
        {
            Gender = gender;
            DateOfBirth = dateOfBirth;
            IsAustralianResident = isAustralianResident;
            SmokerStatus = smokerStatus;
            OccupationCode = occupationCode;
            Income = income;
            IndustryCode = industryCode;
        }        
    }
}
