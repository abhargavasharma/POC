using System;
using System.Configuration;

namespace TAL.QuoteAndApply.Infrastructure.Features
{
    public interface IFeatures
    {
        bool ValidateUnderwritingForPolicySubmission { get; }
        bool AdobeLeadsServiceTrigger { get; }
    }

    public class Features : IFeatures
    {
        public bool ValidateUnderwritingForPolicySubmission
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["Feature.ValidateUnderwritingForPolicySubmission"]);
            }
        }

        public bool AdobeLeadsServiceTrigger
        {
            get
            {
                return Convert.ToBoolean(ConfigurationManager.AppSettings["Feature.AdobeLeadsServiceTrigger"]);
            }
        }
    }
}