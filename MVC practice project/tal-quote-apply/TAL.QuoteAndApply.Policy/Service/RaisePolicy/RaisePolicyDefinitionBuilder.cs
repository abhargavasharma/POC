using System;

namespace TAL.QuoteAndApply.Policy.Service.RaisePolicy
{
    public interface IRaisePolicyDefinitionBuilder
    {
        RaisePolicyMessageHeader BuildRaisePolicyMessageHeaderDefinition();

        RaisePolicyPolicyOrderDetails BuildRaisePolicyPolicyOrderDefinition();
    }

    public class RaisePolicyDefinitionBuilder : IRaisePolicyDefinitionBuilder
    {
        public RaisePolicyMessageHeader BuildRaisePolicyMessageHeaderDefinition()
        {
            var raisePolicyMessageHeaderDefinition = new RaisePolicyMessageHeader
            {
                ACORDStandardVersionCode = "AML_1_1_0",
                MessageId = Guid.NewGuid().ToString(),
                CorrelationId = "85A62CB6-31BE-4FED-A9FF-4BFB1AB97025",
                MessageDateTime = DateTime.Today.Date,
                Sender = "SalesPortal",
                Receiver = "IntegrationLayer"
            };
            return raisePolicyMessageHeaderDefinition;
        }

        public RaisePolicyPolicyOrderDetails BuildRaisePolicyPolicyOrderDefinition()
        {
            var raisePolicyPolicyOrder = new RaisePolicyPolicyOrderDetails
            {
                DocumentId = "85A62CB6-31BE-4FED-A9FF-4BFB1AB97025",
                TransactionFunctionCode = "CreateApplication",
                ApplicationTypeCode = "Increase",
                Description = "Personal case (LifeInsured is PolicyOwner), All covers, including prior",
                CaseId = "BCEE125B-1D0C-47AA-BD5B-25BBAD6B1EA3",
                BroadLineOfBusinessCode = "Individual",
                LodgementTypeCode = "Manual"
            };
            return raisePolicyPolicyOrder;
        }
    }
}