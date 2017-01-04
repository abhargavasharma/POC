using System.Collections.Generic;
using System.Web.Http.ModelBinding;
using TAL.QuoteAndApply.SalesPortal.Web.Models.Api;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.Web.Shared.Converters;
using TAL.QuoteAndApply.Web.Shared.Extensions;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services.Converters
{
    public interface ISetInforceResponseConverter
    {
        SetInforceResponse From(RaisePolicySubmissionResult raisePolicySubmissionResult);
    }

    public class SetInforceResponseConverter : ISetInforceResponseConverter
    {
        private readonly IErrorsAndWarningsConverter _errorsAndWarningsConverter;

        public SetInforceResponseConverter(IErrorsAndWarningsConverter errorsAndWarningsConverter)
        {
            _errorsAndWarningsConverter = errorsAndWarningsConverter;
        }

        public SetInforceResponse From(RaisePolicySubmissionResult raisePolicySubmissionResult)
        {
            var sifRisks = new List<SetInforceRiskSectionValidaitonResponse>();

            foreach (var riskSubmissionResult in raisePolicySubmissionResult.PolicySubmissionValidationResult.RiskSectionStatuses)
            {
                var sifSections = new List<SetInforceSectionValidaitonResponse>();

                foreach (var sectionResult in riskSubmissionResult.Sections)
                {
                    var errors = new ModelStateDictionary();
                    var warnings = new ModelStateDictionary();

                    _errorsAndWarningsConverter.MapGenericValidationToModelState(errors, sectionResult.Errors);
                    _errorsAndWarningsConverter.MapGenericValidationToModelState(warnings, sectionResult.Warnings);

                    sifSections.Add(new SetInforceSectionValidaitonResponse()
                    {
                        SectionName = sectionResult.Section.ToString(),
                        Completed = sectionResult.Completed,
                        Errors = errors.ToErrorDictionary(),
                        Warnings = warnings.ToErrorDictionary()
                    });
                }

                sifRisks.Add(new SetInforceRiskSectionValidaitonResponse
                {
                    RiskId = riskSubmissionResult.RiskId,
                    SectionsValidations = sifSections
                });
            }

            return new SetInforceResponse
            {
                RiskValidations = sifRisks,
                Successful = raisePolicySubmissionResult.SubmittedSuccessfully,
                ServerError = raisePolicySubmissionResult.SubmissionAttempted && !raisePolicySubmissionResult.SubmittedSuccessfully
            };
        }
    }
}