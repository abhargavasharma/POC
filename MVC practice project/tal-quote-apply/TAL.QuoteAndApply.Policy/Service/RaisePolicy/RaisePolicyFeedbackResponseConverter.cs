using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TAL.QuoteAndApply.Infrastructure.Extensions;

namespace TAL.QuoteAndApply.Policy.Service.RaisePolicy
{
    public interface IRaisePolicyFeedbackResponseConverter
    {
        RaisePolicyFeedbackResult From(CanonicalEvent canonicalEvent);
    }

    public class RaisePolicyFeedbackResponseConverter : IRaisePolicyFeedbackResponseConverter
    {
        public RaisePolicyFeedbackResult From(CanonicalEvent canonicalEvent)
        {
            var result = new RaisePolicyFeedbackResult();

            if (HasError(canonicalEvent))
            {
                var errorDetail = canonicalEvent.EventData.Properties.FirstOrDefault(p => p.Name == "ErrorDetail");

                result.QuoteReferenceNumber = GetQuoteReferenceFromErrorPropertyValues(errorDetail);
                result.ErrorDetail = GetErrorContentFromErrorPropertyValues(errorDetail);
                result.ValidationErrors = GetValidationErrorsFromErrorPropertyValues(errorDetail);

                return result;
            }
            
            result.QuoteReferenceNumber = GetQuoteReference(canonicalEvent);
            result.PasUploadId = GetPasPolicyNumber(canonicalEvent);

            return result;
        }

        private string GetPasPolicyNumber(CanonicalEvent canonicalEvent)
        {
            var pasUploadIdNode = canonicalEvent.EventData.Properties.FirstOrDefault(p => p.Name == "PAS_upload_id");
            var pasNumberNode = pasUploadIdNode?.Value.Any.FirstOrDefault();

            if (pasNumberNode != null)
            {
                foreach (XmlNode node in pasNumberNode.ChildNodes)
                {
                    return node.InnerText;
                }
            }
            

            return null;
        }

        private string GetQuoteReference(CanonicalEvent canonicalEvent)
        {
            var appRefNode = canonicalEvent.EventData.Properties.FirstOrDefault(p => p.Name == "ApplicationRefNumber");
            var quoteRefNode = appRefNode?.Value.Any.FirstOrDefault();

            if (quoteRefNode != null)
            {
                foreach (XmlNode node in quoteRefNode.ChildNodes)
                {
                    return node.InnerText;
                }
            }

            return null;
        }

        private bool HasError(CanonicalEvent canonicalEvent)
        {
            return canonicalEvent.EventData.Properties.Any(p => p.Name == "ErrorType");
        }

        public string GetQuoteReferenceFromErrorPropertyValues(CanonicalEventEventDataProperty property)
        {
            var node = property.Value.Any.FirstOrDefault(xml => xml.Name == "ApplicationRefNumber");
            return node?.InnerText;
        }

        public string GetErrorContentFromErrorPropertyValues(CanonicalEventEventDataProperty property)
        {
            var node = property.Value.Any.FirstOrDefault(xml => xml.Name == "Content");
            return node?.InnerText;
        }

        public IEnumerable<string> GetValidationErrorsFromErrorPropertyValues(CanonicalEventEventDataProperty property)
        {
            var validationErrorsNodes = property.Value.Any.FirstOrDefault(xml => xml.Name == "ValidationErrors");

            if (validationErrorsNodes != null)
            {
                var errors = new List<string>();
                foreach (XmlNode node in validationErrorsNodes.ChildNodes)
                {
                    if (node.Name == "ValidationError")
                    {
                        errors.Add(node.InnerText);
                    }
                }

                return errors;
            }

            return null;
        }
    }

    public class RaisePolicyFeedbackResult
    {
        public string QuoteReferenceNumber { get; set; }
        public string PasUploadId { get; set; }
        public string ErrorDetail { get; set; }
        public IEnumerable<string> ValidationErrors { get; set; }

        public RaisePolicyFeedbackResult()
        {
            ValidationErrors = new List<string>();
        }

        public bool HasErrors
        {
            get { return !string.IsNullOrWhiteSpace(ErrorDetail) || ValidationErrors.Any(); }
        }
    }
}
