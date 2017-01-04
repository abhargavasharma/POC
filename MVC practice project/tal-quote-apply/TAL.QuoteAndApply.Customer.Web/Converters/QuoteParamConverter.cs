using System;
using TAL.QuoteAndApply.Customer.Web.Models.View;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.Customer.Web.Converters
{
    public interface IQuoteParamConverter
    {
        CreateQuoteParam FromBasicInfoViewModel(BasicInfoViewModel basicInfoViewModel);
        DateTime FromDateString(string date);
    }

    public class QuoteParamConverter : IQuoteParamConverter
    {
        private readonly IPostcodeService _postcodeService;

        public QuoteParamConverter(IPostcodeService postcodeService)
        {
            _postcodeService = postcodeService;
        }

        public CreateQuoteParam FromBasicInfoViewModel(BasicInfoViewModel basicInfoViewModel)
        {
            var aussieState = _postcodeService.GetStateForPostcode(basicInfoViewModel.Postcode);

            var createQuoteParam = new CreateQuoteParam(
                new RatingFactorsParam(basicInfoViewModel.Gender.Value,
                    FromDateString(basicInfoViewModel.DateOfBirth),
                    null,
                    new SmokerStatusHelper(basicInfoViewModel.IsSmoker.Value),
                    basicInfoViewModel.OccupationCode,
                    basicInfoViewModel.IndustryCode,
                    basicInfoViewModel.AnnualIncome.GetValueOrDefault(0)
                    ),
                new PersonalInformationParam(basicInfoViewModel.Postcode, aussieState.ToString()),
                false,
                basicInfoViewModel.Source, "TAL");

            return createQuoteParam;
        }

        public DateTime FromDateString(string date)
        {
            return date.ToDateExcactDdMmYyyy().Value;
        }
    }
}