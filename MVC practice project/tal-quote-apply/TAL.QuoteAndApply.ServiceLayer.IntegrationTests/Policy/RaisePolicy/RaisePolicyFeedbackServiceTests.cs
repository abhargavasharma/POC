using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy;

namespace TAL.QuoteAndApply.ServiceLayer.IntegrationTests.Policy.RaisePolicy
{
    [TestFixture]
    public  class RaisePolicyFeedbackServiceTests : BaseServiceLayerTest
    {
        [SetUp]
        public void Setup()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();
        }

        [Test]
        public void ProcessRaisePolicyFeedback_SuccesfulPayloadSent_AuditWrittenAndPolicyUpdated()
        {
            var quote = CreateQuoteWithDefaults();

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
          <Value xmlns="""">{quote.QuoteReference}</Value>
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

            var service = GetServiceInstance<IRaisePolicyFeedbackService>();
            service.ProcessRaisePolicyFeedback(rawXml);

            var policyWithRisks = GetPolicy(quote.QuoteReference);

            Assert.That(policyWithRisks.Policy.Status, Is.EqualTo(PolicyStatus.Inforce));
            
            var auditRepo = GetServiceInstance<IRaisePolicySubmissionAuditDtoRepository>();
            var audits = auditRepo.GetAllAuditsForPolicy(policyWithRisks.Policy.Id);

            var thisAudit =
                audits.FirstOrDefault(
                    a => a.RaisePolicyAuditType == RaisePolicyAuditType.Feedback && a.RaisePolicyXml == rawXml);

            Assert.That(thisAudit, Is.Not.Null);
        }

        [Test]
        public void ProcessRaisePolicyFeedback_ValidationErrorPayloadSent_AuditWrittenAndPolicyUpdated()
        {
            var quote = CreateQuoteWithDefaults();
            
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
          <ApplicationRefNumber xmlns="""">{quote.QuoteReference}</ApplicationRefNumber>
          <ValidationErrors xmlns="""">
            <ValidationError>SOMETHING WENT WRONG</ValidationError>
          </ValidationErrors>
        </Value>
      </Property>
    </Properties>
  </EventData>
</CanonicalEvent>";

            var service = GetServiceInstance<IRaisePolicyFeedbackService>();
            service.ProcessRaisePolicyFeedback(rawXml);

            var policyWithRisks = GetPolicy(quote.QuoteReference);

            Assert.That(policyWithRisks.Policy.Status, Is.EqualTo(PolicyStatus.FailedDuringPolicyAdminSystemLoad));
            
            var auditRepo = GetServiceInstance<IRaisePolicySubmissionAuditDtoRepository>();
            var audits = auditRepo.GetAllAuditsForPolicy(policyWithRisks.Policy.Id);

            var thisAudit =
                audits.FirstOrDefault(
                    a => a.RaisePolicyAuditType == RaisePolicyAuditType.Feedback && a.RaisePolicyXml == rawXml);

            Assert.That(thisAudit, Is.Not.Null);
        }

        [Test]
        public void ProcessRaisePolicyFeedback_ErrorPayloadSent_AuditWrittenAndPolicyUpdated()
        {
            var quote = CreateQuoteWithDefaults();

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
          <ApplicationRefNumber xmlns="""">{quote.QuoteReference}</ApplicationRefNumber>
          <Content xmlns="""">Unable to write staging data: ORA-12504: TNS:listener was not given the SERVICE_NAME in CONNECT_DATA</Content>
        </Value>
      </Property>
    </Properties>
  </EventData>
</CanonicalEvent>";

            var service = GetServiceInstance<IRaisePolicyFeedbackService>();
            service.ProcessRaisePolicyFeedback(rawXml);

            var policyWithRisks = GetPolicy(quote.QuoteReference);

            Assert.That(policyWithRisks.Policy.Status, Is.EqualTo(PolicyStatus.FailedDuringPolicyAdminSystemLoad));
            
            var auditRepo = GetServiceInstance<IRaisePolicySubmissionAuditDtoRepository>();
            var audits = auditRepo.GetAllAuditsForPolicy(policyWithRisks.Policy.Id);

            var thisAudit =
                audits.FirstOrDefault(
                    a => a.RaisePolicyAuditType == RaisePolicyAuditType.Feedback && a.RaisePolicyXml == rawXml);

            Assert.That(thisAudit, Is.Not.Null);
        }
    }
}
