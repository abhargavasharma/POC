using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Underwriting.Configuration;
using TAL.QuoteAndApply.Underwriting.Models;
using TAL.QuoteAndApply.Underwriting.Models.Talus;

namespace TAL.QuoteAndApply.Underwriting.Service
{
    public interface ICreateInterviewRequestProvider
    {
        CreateInterviewRequest Create(string templateName, string workflow, IEnumerable<string> benefitCodes, string createdBy);
    }

    public class CreateInterviewRequestProvider : ICreateInterviewRequestProvider
    {
        public CreateInterviewRequest Create(string templateName, string workflow, IEnumerable<string> benefitCodes, string createdBy)
        {
            return new CreateInterviewRequest
            {
                TemplateType = templateName,
                CreatedBy = createdBy,
                Benefits = benefitCodes.Select(b => new CreateInterviewRequestBenefits
                {
                    Benefit = b,
                    Workflow = workflow
                }).ToArray()
            };
        }
    }

}
