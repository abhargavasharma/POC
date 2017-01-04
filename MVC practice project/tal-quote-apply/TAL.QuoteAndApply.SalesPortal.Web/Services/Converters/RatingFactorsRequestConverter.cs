using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.SalesPortal.Web.Extensions;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface IRatingFactorsRequestConverter
    {
        RatingFactorsRequest From(RatingFactorsResult ratingFactors);
        RatingFactorsParam From(RatingFactorsRequest ratingFactorsRequest);
    }
    public class RatingFactorsRequestConverter : IRatingFactorsRequestConverter
    {
        public RatingFactorsRequest From(RatingFactorsResult ratingFactors)
        {
            return new RatingFactorsRequest()
            {
                Gender = ratingFactors.Gender,
                DateOfBirth = ratingFactors.DateOfBirth.ToFriendlyString(),
                AustralianResident = ratingFactors.IsAustralianResident,
                Income = ratingFactors.Income,
                OccupationCode = ratingFactors.OccupationCode,
                OccupationTitle = ratingFactors.OccupationTitle,
                IndustryCode = ratingFactors.IndustryCode,
                IndustryTitle = ratingFactors.IndustryTitle,
                SmokerStatus = ratingFactors.SmokerStatus
            };
        }

        public RatingFactorsParam From(RatingFactorsRequest ratingFactorsRequest)
        {
            return new RatingFactorsParam(
                ratingFactorsRequest.Gender.Value,
                ratingFactorsRequest.DateOfBirth.ToDateExcactDdMmYyyy().Value,
                ratingFactorsRequest.AustralianResident.Value,
                new SmokerStatusHelper(ratingFactorsRequest.SmokerStatus),
                ratingFactorsRequest.OccupationCode,
                ratingFactorsRequest.IndustryCode,
                ratingFactorsRequest.Income.GetValueOrDefault(0));
        }
    }
}