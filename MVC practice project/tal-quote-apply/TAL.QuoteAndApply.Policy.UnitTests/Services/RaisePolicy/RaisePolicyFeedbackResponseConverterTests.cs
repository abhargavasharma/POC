using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Policy.Service.RaisePolicy;

namespace TAL.QuoteAndApply.Policy.UnitTests.Services.RaisePolicy
{
    [TestFixture]
    public class RaisePolicyFeedbackResponseConverterTests
    {
        [Test]
        public void From_SuccessfulResponse()
        {
            var quoteReferenceNumber = "M123456789";
            var pasPolicyNumber = "PAS1234567";

            var rawXml = 
$@"<CanonicalEvent xmlns=""TAL.Common"">
  <EventInfo>
    <ProcessName>TAL.Integration.PAS.Policy.Orchestrations.SubmitApplication</ProcessName>
    <TrackingId>cba6660780634eb19fae2957b4d4eef4</TrackingId>
    <Sender>IntegrationLayer</Sender>
    <Date>2016-06-27T09:29:49.0021831+10:00</Date>
    <Type>Error</Type>
  </EventInfo>
  <EventData>
    <Properties>
      <Property>
        <Name>ServiceCall</Name>
        <Value>Upload</Value>
      </Property>
      <Property>
        <Name>ApplicationRefNumber</Name>
        <Value>
          <Value xmlns="""">{quoteReferenceNumber}</Value>
        </Value>
      </Property>
      <Property>
        <Name>PAS_upload_id</Name>
        <Value>
          <Value xmlns="""">{pasPolicyNumber}</Value>
        </Value>
      </Property>
    </Properties>
  </EventData>
</CanonicalEvent>";

            var canonicalEvent = rawXml.FromXml<CanonicalEvent>();

            var converter = new RaisePolicyFeedbackResponseConverter();
            var feedbackResult = converter.From(canonicalEvent);

            Assert.That(feedbackResult, Is.Not.Null);
            Assert.That(feedbackResult.QuoteReferenceNumber, Is.EqualTo(quoteReferenceNumber));
            Assert.That(feedbackResult.PasUploadId, Is.EqualTo(pasPolicyNumber));
            Assert.That(feedbackResult.ErrorDetail, Is.Null);
        }

        [Test]
        public void From_ValidationErrorResponse()
        {
            var quoteReferenceNumber = "M123456789";
            var validationError = "INSURED_BENEFIT_STG:MONTHLY_PREMIUM length exceeds maximum length of 12";
            var rawXml =
$@"<CanonicalEvent xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""TAL.Common"">
  <EventData>
    <Properties>
      <Property>
        <Name>ErrorType</Name>
        <Value>
          <ValidationError xmlns="""" />
        </Value>
      </Property>
      <Property>
        <Name>ErrorDetail</Name>
        <Value>
          <ApplicationRefNumber xmlns="""">{quoteReferenceNumber}</ApplicationRefNumber>
          <ValidationErrors xmlns="""">
            <ValidationError>{validationError}</ValidationError>
          </ValidationErrors>
        </Value>
      </Property>
    </Properties>
  </EventData>
</CanonicalEvent>";

            var canonicalEvent = rawXml.FromXml<CanonicalEvent>();

            var converter = new RaisePolicyFeedbackResponseConverter();
            var feedbackResult = converter.From(canonicalEvent);

            Assert.That(feedbackResult, Is.Not.Null);
            Assert.That(feedbackResult.QuoteReferenceNumber, Is.EqualTo(quoteReferenceNumber));
            Assert.That(feedbackResult.PasUploadId, Is.Null);
            Assert.That(feedbackResult.ErrorDetail, Is.Null);
            Assert.That(feedbackResult.ValidationErrors, Is.Not.Null);
            Assert.That(feedbackResult.ValidationErrors.Any(), Is.True);
            Assert.That(feedbackResult.ValidationErrors.First(), Is.EqualTo(validationError));
        }

        [Test]
        public void From_ServiceErrorResponse()
        {
            var quoteReferenceNumber = "M123456789";
            var serviceError = "Unable to write staging data: ORA-12504: TNS:listener was not given the SERVICE_NAME in CONNECT_DATA";
            var rawXml =
$@"<CanonicalEvent xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""TAL.Common"">
  <EventData>
    <Properties>
      <Property>
        <Name>ErrorType</Name>
        <Value>
          <ProcessError xmlns="""" />
        </Value>
      </Property>
      <Property>
        <Name>ErrorDetail</Name>
        <Value>
          <ApplicationRefNumber xmlns="""">{quoteReferenceNumber}</ApplicationRefNumber>
          <Content xmlns="""">Unable to write staging data: ORA-12504: TNS:listener was not given the SERVICE_NAME in CONNECT_DATA</Content>
        </Value>
      </Property>
    </Properties>
  </EventData>
</CanonicalEvent>";

            var canonicalEvent = rawXml.FromXml<CanonicalEvent>();

            var converter = new RaisePolicyFeedbackResponseConverter();
            var feedbackResult = converter.From(canonicalEvent);

            Assert.That(feedbackResult, Is.Not.Null);
            Assert.That(feedbackResult.QuoteReferenceNumber, Is.EqualTo(quoteReferenceNumber));
            Assert.That(feedbackResult.PasUploadId, Is.Null);
            Assert.That(feedbackResult.ErrorDetail, Is.EqualTo(serviceError));
        }

        [Test]
        public void From_DuplicateErrorResponse()
        {
            var quoteReferenceNumber = "M123456789";
            var duplicateError = @"Unable to write upload data: ORA-20222: Failed to create NEWAPP upload item ORA-06512: at OPS$PROFAI.SUBMITAPP, line 35 ORA-06512: at line 1";
            var rawXml =
$@"<CanonicalEvent xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns=""TAL.Common"">
    <EventData>
        <Properties>
            <Property>
              <Name>ErrorType</Name>
              <Value>
                  <ProcessError xmlns="""" />
              </Value>
          </Property>
          <Property>
            <Name>ErrorDetail</Name>
            <Value>
                <ApplicationRefNumber xmlns="""">{quoteReferenceNumber}</ApplicationRefNumber>
                <Content>{duplicateError}</Content>
            </Value>
        </Property>
    </Properties>
  </EventData>
</CanonicalEvent>";

            var canonicalEvent = rawXml.FromXml<CanonicalEvent>();

            var converter = new RaisePolicyFeedbackResponseConverter();
            var feedbackResult = converter.From(canonicalEvent);

            Assert.That(feedbackResult, Is.Not.Null);
            Assert.That(feedbackResult.QuoteReferenceNumber, Is.EqualTo(quoteReferenceNumber));
            Assert.That(feedbackResult.PasUploadId, Is.Null);
            Assert.That(feedbackResult.ErrorDetail, Is.EqualTo(duplicateError));
        }
    }
}
