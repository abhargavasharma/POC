using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Personal;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models;
using CreditCardType = TAL.QuoteAndApply.DataModel.Payment.CreditCardType;

namespace TAL.QuoteAndApply.ServiceLayer.IntegrationTests.Policy.RaisePolicy
{
    [TestFixture]
    public class RaisePolicyConverterTests : BaseServiceLayerTest
    {
        [Test]
        public void From_VerifyOccupationClass_SetOnPolicyNewBusinessOrder()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var raisePolicyBuilder = new RaisePolicyBuilder();
            var raisePolicy = raisePolicyBuilder.GetBaseRaisePolicy();
            raisePolicy = raisePolicyBuilder.AddRiderWithAllOptions(raisePolicy);
            raisePolicy = raisePolicyBuilder.AddCreditCardPayment(raisePolicy);

            var converter = GetServiceInstance<IRaisePolicyConverter>();
            var result = converter.From(raisePolicy);

            var person = result.PolicyOrder.Person[1]; // life insured is number 2 now
            var occupation = person.Occupation.First();
            var occupationClassCode = occupation.OccupationClassCode.First();

            Assert.That(occupationClassCode.Value.Name, Is.EqualTo(raisePolicy.PrimaryRisk.PasCode));
        }

        [Test]
        public void From_VerifyCreditCard_SetOnPolicyNewBusinessOrder()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var raisePolicyBuilder = new RaisePolicyBuilder();
            var raisePolicy = raisePolicyBuilder.GetBaseRaisePolicy();
            raisePolicy = raisePolicyBuilder.AddRiderWithAllOptions(raisePolicy);
            raisePolicy = raisePolicyBuilder.AddCreditCardPayment(raisePolicy);

            var converter = GetServiceInstance<IRaisePolicyConverter>();
            var result = converter.From(raisePolicy);

            var tokenNode =
                result.PolicyOrder.Policy.PolicySection.AssignedIdentifier.FirstOrDefault(
                    x => x.IdentifierDescription == "CreditCardToken");

            Assert.That(tokenNode, Is.Not.Null);
            Assert.That(tokenNode.Id, Is.EqualTo(raisePolicy.Payment.CreditCard.Token.Replace("-", "")));

            Console.WriteLine(raisePolicy.Payment.CreditCard.Token);
            Console.WriteLine(tokenNode.Id);

            Assert.That(
                result.PolicyOrder.Policy.PolicySection.PolicyBilling.BankAccount[0].AccountNumberId,
                Is.EqualTo(raisePolicy.Payment.CreditCard.CardNumber));
        }

        [Test]
        public void From_VerifyAgent_SetOnPolicyNewBusinessOrder()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var raisePolicyBuilder = new RaisePolicyBuilder();
            var raisePolicy = raisePolicyBuilder.GetBaseRaisePolicy();
            raisePolicy = raisePolicyBuilder.AddRiderWithAllOptions(raisePolicy);
            raisePolicy = raisePolicyBuilder.AddCreditCardPayment(raisePolicy);

            var converter = GetServiceInstance<IRaisePolicyConverter>();
            var result = converter.From(raisePolicy);

            var agentInterestedParty =
                result.PolicyOrder.Policy.PolicySection.PolicyInterestedParty.FirstOrDefault(
                    p => p.key == "AGENT0");
            Assert.That(agentInterestedParty, Is.Not.Null);
            Assert.That(agentInterestedParty.RoleCode.Value.Name, Is.EqualTo("Agency"));

            var organisation = result.PolicyOrder.Organization.First();
            Assert.That(organisation.key, Is.EqualTo("AGENT0"));
            Assert.That(organisation.AssignedIdentifier.First().RoleCode.Value.Name, Is.EqualTo("Agency"));
            Assert.That(organisation.AssignedIdentifier.First().Id, Is.EqualTo("51243"));
        }

        [Test]
        public void From_VerifyCommisionsAndFees_SetOnPolicyNewBusinessOrder()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var raisePolicyBuilder = new RaisePolicyBuilder();
            var raisePolicy = raisePolicyBuilder.GetBaseRaisePolicy();
            raisePolicy = raisePolicyBuilder.AddRiderWithAllOptions(raisePolicy);
            raisePolicy = raisePolicyBuilder.AddCreditCardPayment(raisePolicy);

            var converter = GetServiceInstance<IRaisePolicyConverter>();
            var result = converter.From(raisePolicy);

            var insuranceAmountItem =
                result.PolicyOrder.Policy.PolicySection.InsuranceAmountItem.First();

            var policyFee =
                insuranceAmountItem.ContractAmountItem.FirstOrDefault(x => x.TypeCode.Value.Name == "PolicyFee");
            Assert.That(policyFee, Is.Not.Null);
            Assert.That(policyFee.ItemAmount.Value, Is.EqualTo(0));

            var newBusinessCommission =
                insuranceAmountItem.ContractAmountItem.FirstOrDefault(
                    x => x.TypeCode.Value.Name == "NewBusinessCommission");
            Assert.That(newBusinessCommission, Is.Not.Null);
            Assert.That(newBusinessCommission.Commission.CommissionRatePercent, Is.EqualTo(100));

            var servicingCommission =
                insuranceAmountItem.ContractAmountItem.FirstOrDefault(
                    x => x.TypeCode.Value.Name == "ServicingCommission");
            Assert.That(servicingCommission, Is.Not.Null);
            Assert.That(servicingCommission.Commission.CommissionRatePercent, Is.EqualTo(100));
        }

        [Test]
        public void From_VerifyLifePlan_SetOnPolicyNewBusinessOrder()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var raisePolicyBuilder = new RaisePolicyBuilder();
            var raisePolicy = raisePolicyBuilder.GetBaseRaisePolicy();
            raisePolicy = raisePolicyBuilder.AddLifePlan(raisePolicy);
            raisePolicy = raisePolicyBuilder.AddCreditCardPayment(raisePolicy);

            var converter = GetServiceInstance<IRaisePolicyConverter>();
            var result = converter.From(raisePolicy);

            var coverages =
                result.PolicyOrder.Policy.LifeItemSection.First().Coverage;

            var lifeCoverage = coverages.FirstOrDefault(x => x.TypeCode.Value.Name == "Death");
            Assert.That(lifeCoverage, Is.Not.Null);
            Assert.That(lifeCoverage.UnderwritingDecision, Is.EqualTo("Accepted"));
            Assert.That(lifeCoverage.CoverageCategory.CoverageCategoryCode.Value.Name, Is.EqualTo("Primary"));
            Assert.That(lifeCoverage.Limit.First().ValuationTypeCode.Value.Name, Is.EqualTo("SumInsured"));
            AssertOptionAssignedIdentifier(lifeCoverage.Option, "CommissionStyle", "L");
            AssertOptionAssignedIdentifier(lifeCoverage.Option, "CommissionRate", "STD");
            AssertOptionAssignedIdentifier(lifeCoverage.Option, "OutOfTreatyAdjustmentPercent", "0.00");
            AssertOptionAssignedIdentifier(lifeCoverage.Option, "CPIIndexationFlag", "Y");
            AssertOptionAssignedIdentifier(lifeCoverage.Option, "CoverType", null);

            AssertOptionAssignedIdentifier(lifeCoverage.Option, "AccidentInjuryCoverFlag", "Y");
            AssertOptionAssignedIdentifier(lifeCoverage.Option, "IllnessCoverFlag", "Y");
            AssertOptionAssignedIdentifier(lifeCoverage.Option, "SportsCancerFlag", "Y");

            AssertOptionAdded(lifeCoverage.Option, "IndemnityOption");
        }

        [Test]
        public void From_VerifyLifePlanWithAccidentCoverBlockSwitchedOff_SetOnPolicyNewBusinessOrder()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var raisePolicyBuilder = new RaisePolicyBuilder();
            var raisePolicy = raisePolicyBuilder.GetBaseRaisePolicy();
            raisePolicy = raisePolicyBuilder.AddLifePlan(raisePolicy);
            raisePolicy = raisePolicyBuilder.TurnLifeAccidentCoverBlockOff(raisePolicy);
            raisePolicy = raisePolicyBuilder.AddCreditCardPayment(raisePolicy);

            var converter = GetServiceInstance<IRaisePolicyConverter>();
            var result = converter.From(raisePolicy);

            var coverages =
                result.PolicyOrder.Policy.LifeItemSection.First().Coverage;

            var lifeCoverage = coverages.FirstOrDefault(x => x.TypeCode.Value.Name == "Death");
            Assert.That(lifeCoverage, Is.Not.Null);
            AssertOptionAssignedIdentifier(lifeCoverage.Option, "AccidentInjuryCoverFlag", "N");
            AssertOptionAssignedIdentifier(lifeCoverage.Option, "IllnessCoverFlag", "Y");
            AssertOptionAssignedIdentifier(lifeCoverage.Option, "SportsCancerFlag", "Y");
        }

        [Test]
        public void From_VerifyTpdRiderPlan_SetOnPolicyNewBusinessOrder()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var raisePolicyBuilder = new RaisePolicyBuilder();
            var raisePolicy = raisePolicyBuilder.GetBaseRaisePolicy();
            raisePolicy = raisePolicyBuilder.AddRiderWithAllOptions(raisePolicy);
            raisePolicy = raisePolicyBuilder.AddCreditCardPayment(raisePolicy);

            var converter = GetServiceInstance<IRaisePolicyConverter>();
            var result = converter.From(raisePolicy);

            var coverages =
                result.PolicyOrder.Policy.LifeItemSection.First().Coverage;

            var tpdRiderCoverage = coverages.FirstOrDefault(x => x.TypeCode.Value.Name == "TotalAndPermanentDisability");
            Assert.That(tpdRiderCoverage, Is.Not.Null);
            Assert.That(tpdRiderCoverage.UnderwritingDecision, Is.EqualTo("Accepted"));
            Assert.That(tpdRiderCoverage.CoverageCategory.CoverageCategoryCode.Value.Name, Is.EqualTo("Rider"));
            Assert.That(tpdRiderCoverage.Limit.First().ValuationTypeCode.Value.Name, Is.EqualTo("SumInsured"));
            AssertOptionAssignedIdentifier(tpdRiderCoverage.Option, "CommissionStyle", "L");
            AssertOptionAssignedIdentifier(tpdRiderCoverage.Option, "CommissionRate", "STD");
            AssertOptionAssignedIdentifier(tpdRiderCoverage.Option, "OutOfTreatyAdjustmentPercent", "0.00");
            AssertOptionAssignedIdentifier(tpdRiderCoverage.Option, "CPIIndexationFlag", "N");
            AssertOptionAssignedIdentifier(tpdRiderCoverage.Option, "CoverType", null);
            AssertOptionAdded(tpdRiderCoverage.Option, "PremiumRelief");
            AssertOptionAdded(tpdRiderCoverage.Option, "DeathBuyBack");
            AssertOptionAdded(tpdRiderCoverage.Option, "DayOneAccidentBenefit");
            AssertOptionAdded(tpdRiderCoverage.Option, "IndemnityOption");
            AssertOptionAddedAndValue(tpdRiderCoverage.Option, "TPDDefinition", "OwnOccupation");
        }

        [Test]
        public void From_VerifyIpPlan_SetOnPolicyNewBusinessOrder()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var raisePolicyBuilder = new RaisePolicyBuilder();
            var raisePolicy = raisePolicyBuilder.GetBaseRaisePolicy();
            raisePolicy = raisePolicyBuilder.AddIpPlan(raisePolicy);
            raisePolicy = raisePolicyBuilder.AddCreditCardPayment(raisePolicy);

            var converter = GetServiceInstance<IRaisePolicyConverter>();
            var result = converter.From(raisePolicy);

            var coverages =
                result.PolicyOrder.Policy.LifeItemSection.First().Coverage;

            var ipCoverage = coverages.FirstOrDefault(x => x.TypeCode.Value.Name == "IncomeProtection");
            Assert.That(ipCoverage, Is.Not.Null);
            Assert.That(ipCoverage.UnderwritingDecision, Is.EqualTo("Accepted"));
            Assert.That(ipCoverage.CoverageCategory.CoverageCategoryCode.Value.Name, Is.EqualTo("Primary"));
            Assert.That(ipCoverage.Limit.First().ValuationTypeCode.Value.Name, Is.EqualTo("MonthlyBenefit"));
            Assert.That(ipCoverage.BenefitPeriodCode.Value.Name, Is.EqualTo("Year1"));
            Assert.That(ipCoverage.WaitingPeriodCode.Value.Name, Is.EqualTo("Day28"));

            AssertOptionAssignedIdentifier(ipCoverage.Option, "CommissionStyle", "L");
            AssertOptionAssignedIdentifier(ipCoverage.Option, "CommissionRate", "STD");
            AssertOptionAssignedIdentifier(ipCoverage.Option, "CoverType", "S");
            AssertOptionAssignedIdentifier(ipCoverage.Option, "OutOfTreatyAdjustmentPercent", "0.00");
            AssertOptionAssignedIdentifier(ipCoverage.Option, "CPIIndexationFlag", "N");

            AssertOptionAssignedIdentifier(ipCoverage.Option, "AccidentInjuryCoverFlag", "Y");
            AssertOptionAssignedIdentifier(ipCoverage.Option, "IllnessCoverFlag", "Y");
            AssertOptionAssignedIdentifier(ipCoverage.Option, "SportsCancerFlag", "Y");

            AssertOptionAdded(ipCoverage.Option, "IndemnityOption");
        }

        [Test]
        public void From_VerifyCiPlan_SetOnPolicyNewBusinessOrder()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var raisePolicyBuilder = new RaisePolicyBuilder();
            var raisePolicy = raisePolicyBuilder.GetBaseRaisePolicy();
            raisePolicy = raisePolicyBuilder.AddCiPlan(raisePolicy);
            raisePolicy = raisePolicyBuilder.AddCreditCardPayment(raisePolicy);

            var converter = GetServiceInstance<IRaisePolicyConverter>();
            var result = converter.From(raisePolicy);

            var coverages =
                result.PolicyOrder.Policy.LifeItemSection.First().Coverage;

            var ciCoverage = coverages.FirstOrDefault(x => x.TypeCode.Value.Name == "CriticalIllness");
            Assert.That(ciCoverage, Is.Not.Null);
            Assert.That(ciCoverage.UnderwritingDecision, Is.EqualTo("Accepted"));
            Assert.That(ciCoverage.CoverageCategory.CoverageCategoryCode.Value.Name, Is.EqualTo("Primary"));
            Assert.That(ciCoverage.Limit.First().ValuationTypeCode.Value.Name, Is.EqualTo("SumInsured"));
            AssertOptionAssignedIdentifier(ciCoverage.Option, "CoverType", "S");
            AssertOptionAssignedIdentifier(ciCoverage.Option, "AccidentInjuryCoverFlag", "Y");
            AssertOptionAssignedIdentifier(ciCoverage.Option, "IllnessCoverFlag", "N");
            AssertOptionAssignedIdentifier(ciCoverage.Option, "SportsCancerFlag", "N");
        }

        [Test]
        public void From_NoReferralsOnPolicy_VerifiySubmittedDateSetAsUnderwritingDecisionDate()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var raisePolicyBuilder = new RaisePolicyBuilder();
            var raisePolicy = raisePolicyBuilder.GetBaseRaisePolicy();
            raisePolicy = raisePolicyBuilder.AddLifePlan(raisePolicy);
            raisePolicy = raisePolicyBuilder.AddCreditCardPayment(raisePolicy);

            var converter = GetServiceInstance<IRaisePolicyConverter>();
            var result = converter.From(raisePolicy);

            var coverages =
                result.PolicyOrder.Policy.LifeItemSection.First().Coverage;

            var lifeCoverage = coverages.FirstOrDefault(x => x.TypeCode.Value.Name == "Death");
            Assert.That(lifeCoverage, Is.Not.Null);

            var uwDesicionDate = lifeCoverage.PolicyReferences.AssignedIdentifier.FirstOrDefault(
                x => x.IdentifierDescription == "UnderwritingDecisionDate");

            Assert.That(uwDesicionDate, Is.Not.Null);
            Assert.That(uwDesicionDate.Id, Is.EqualTo(raisePolicy.ReadyToSubmitDateTime.ToString("yyyy-MM-dd")));
        }

        [Test]
        public void From_ReferralsOnPolicy_VerifiyLastCompletedReferralSetAsUnderwritingDecisionDate()
        {
            var lastCompletedReferral = DateTime.Now.AddDays(-2);

            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var raisePolicyBuilder = new RaisePolicyBuilder();
            var raisePolicy = raisePolicyBuilder.GetBaseRaisePolicy();
            raisePolicy = raisePolicyBuilder.AddLifePlan(raisePolicy);
            raisePolicy = raisePolicyBuilder.AddCreditCardPayment(raisePolicy);
            raisePolicy = raisePolicyBuilder.AddLastCompletedReferral(raisePolicy, lastCompletedReferral);

            var converter = GetServiceInstance<IRaisePolicyConverter>();
            var result = converter.From(raisePolicy);

            var coverages =
                result.PolicyOrder.Policy.LifeItemSection.First().Coverage;

            var lifeCoverage = coverages.FirstOrDefault(x => x.TypeCode.Value.Name == "Death");
            Assert.That(lifeCoverage, Is.Not.Null);

            var uwDesicionDate = lifeCoverage.PolicyReferences.AssignedIdentifier.FirstOrDefault(
                x => x.IdentifierDescription == "UnderwritingDecisionDate");

            Assert.That(uwDesicionDate, Is.Not.Null);
            Assert.That(uwDesicionDate.Id, Is.EqualTo(lastCompletedReferral.ToString("yyyy-MM-dd")));
        }

        [Test]
        public void From_PolicyHasNoLoadingsOrExclusions_VerifyPolicyUnderwritingOutcomeIsStandard()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var raisePolicyBuilder = new RaisePolicyBuilder();
            var raisePolicy = raisePolicyBuilder.GetBaseRaisePolicy();
            raisePolicy = raisePolicyBuilder.AddLifePlan(raisePolicy);
            raisePolicy = raisePolicyBuilder.AddCreditCardPayment(raisePolicy);

            var converter = GetServiceInstance<IRaisePolicyConverter>();
            var result = converter.From(raisePolicy);

            var uwOutcome =
                result.PolicyOrder.PolicyReferences.AssignedIdentifier.First(
                    x => x.IdentifierDescription == "UnderwritingOutcome");

            Assert.That(uwOutcome, Is.Not.Null);
            Assert.That(uwOutcome.Id, Is.EqualTo("Standard"));
        }

        [Test]
        public void From_PolicyHasLoading_VerifyPolicyUnderwritingOutcomeIsStandard()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var raisePolicyBuilder = new RaisePolicyBuilder();
            var raisePolicy = raisePolicyBuilder.GetBaseRaisePolicy();
            raisePolicy = raisePolicyBuilder.AddLifePlan(raisePolicy);
            raisePolicy = raisePolicyBuilder.AddPerMilleLoadingToLifePlanAccidentCover(raisePolicy, 2);
            raisePolicy = raisePolicyBuilder.AddCreditCardPayment(raisePolicy);

            var converter = GetServiceInstance<IRaisePolicyConverter>();
            var result = converter.From(raisePolicy);

            var uwOutcome =
                result.PolicyOrder.PolicyReferences.AssignedIdentifier.First(
                    x => x.IdentifierDescription == "UnderwritingOutcome");

            Assert.That(uwOutcome, Is.Not.Null);
            Assert.That(uwOutcome.Id, Is.EqualTo("Revised"));
        }

        [Test]
        public void From_PolicyHasExclusion_VerifyPolicyUnderwritingOutcomeIsStandard()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var raisePolicyBuilder = new RaisePolicyBuilder();
            var raisePolicy = raisePolicyBuilder.GetBaseRaisePolicy();
            raisePolicy = raisePolicyBuilder.AddLifePlan(raisePolicy);
            raisePolicy = raisePolicyBuilder.AddExclusionToLifePlanAccidentCover(raisePolicy);
            raisePolicy = raisePolicyBuilder.AddCreditCardPayment(raisePolicy);

            var converter = GetServiceInstance<IRaisePolicyConverter>();
            var result = converter.From(raisePolicy);

            var uwOutcome =
                result.PolicyOrder.PolicyReferences.AssignedIdentifier.First(
                    x => x.IdentifierDescription == "UnderwritingOutcome");

            Assert.That(uwOutcome, Is.Not.Null);
            Assert.That(uwOutcome.Id, Is.EqualTo("Revised"));
        }

        [Test]
        public void From_CoverWithPerMilleAndPercentageLoadings_VerifyLoadingsForCover()
        {
            decimal perMilleLoadingAmt = 2;
            decimal percentageLoadingAmt = 50;

            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var raisePolicyBuilder = new RaisePolicyBuilder();
            var raisePolicy = raisePolicyBuilder.GetBaseRaisePolicy();
            raisePolicy = raisePolicyBuilder.AddLifePlan(raisePolicy);
            raisePolicy = raisePolicyBuilder.AddPerMilleLoadingToLifePlanAccidentCover(raisePolicy, perMilleLoadingAmt);
            raisePolicy = raisePolicyBuilder.AddPercentageLoadingToLifePlanAccidentCover(raisePolicy, percentageLoadingAmt);
            raisePolicy = raisePolicyBuilder.AddCreditCardPayment(raisePolicy);

            var converter = GetServiceInstance<IRaisePolicyConverter>();
            var result = converter.From(raisePolicy);

            Console.WriteLine(result.ToXml());

            var coverages =
               result.PolicyOrder.Policy.LifeItemSection.First().Coverage;

            var lifeCoverage = coverages.FirstOrDefault(x => x.TypeCode.Value.Name == "Death");
            Assert.That(lifeCoverage, Is.Not.Null);

            var allLoadings =
                lifeCoverage.InsuranceAmountItem.First(x => x.TypeCode != null && x.TypeCode.Value.Name == "Loading");

            var perMilleLoading = allLoadings.ContractAmountItem.First(x => x.Description == "AC" && x.TypeCode.Value.Name == "PM");
            Assert.That(perMilleLoading.ItemAmount.Value, Is.EqualTo(perMilleLoadingAmt));
            Assert.That(perMilleLoading.ItemFactor, Is.EqualTo(0));

            var percentageLoading = allLoadings.ContractAmountItem.First(x => x.Description == "AC" && x.TypeCode.Value.Name == "RI");
            Assert.That(percentageLoading.ItemPercent, Is.EqualTo(percentageLoadingAmt));
            Assert.That(percentageLoading.ItemFactor, Is.EqualTo(0));
        }

        [Test]
        public void From_VerifyPlanPremium()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var raisePolicyBuilder = new RaisePolicyBuilder();
            var raisePolicy = raisePolicyBuilder.GetBaseRaisePolicy();
            raisePolicy = raisePolicyBuilder.AddLifePlan(raisePolicy);
            raisePolicy = raisePolicyBuilder.AddCreditCardPayment(raisePolicy);

            var converter = GetServiceInstance<IRaisePolicyConverter>();
            var result = converter.From(raisePolicy);

            var coverages =
               result.PolicyOrder.Policy.LifeItemSection.First().Coverage;

            var lifeCoverage = coverages.FirstOrDefault(x => x.TypeCode.Value.Name == "Death");
            Assert.That(lifeCoverage, Is.Not.Null);

            var insuredPayableAmmount =
                lifeCoverage.InsuranceAmountItem.First(x => x.TypeCode != null && x.TypeCode.Value.Name == "InsuredPayableAmount");
            
            var premium = insuredPayableAmmount.ContractAmountItem.First(x => x.TypeCode.Value.Name == "TaxablePremium");
            Assert.That(premium.ItemAmount.Value, Is.EqualTo(1000m));
        }

        [Test]
        public void From_VerifyPlanDiscountFactor()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var raisePolicyBuilder = new RaisePolicyBuilder();
            var raisePolicy = raisePolicyBuilder.GetBaseRaisePolicy();
            raisePolicy = raisePolicyBuilder.AddLifePlan(raisePolicy);
            raisePolicy = raisePolicyBuilder.AddCreditCardPayment(raisePolicy);

            var converter = GetServiceInstance<IRaisePolicyConverter>();
            var result = converter.From(raisePolicy);

            var coverages =
               result.PolicyOrder.Policy.LifeItemSection.First().Coverage;

            var lifeCoverage = coverages.FirstOrDefault(x => x.TypeCode.Value.Name == "Death");
            Assert.That(lifeCoverage, Is.Not.Null);

            var discountNode = lifeCoverage.Deductible.First().AssignedIdentifier.First();

            Assert.That(discountNode.Id, Is.EqualTo(.9m.ToString()));
            Assert.That(discountNode.RoleCode.Value.Name, Is.EqualTo("Insurer"));
            Assert.That(discountNode.IdentifierDescription, Is.EqualTo("HealthSenseDiscountPercent"));
        }

        [Test]
        public void From_VerifyDocumentUrl_SetOnPolicyNewBusinessOrder()
        {
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var raisePolicyBuilder = new RaisePolicyBuilder();
            var raisePolicy = raisePolicyBuilder.GetBaseRaisePolicy();
            raisePolicy = raisePolicyBuilder.AddRiderWithAllOptions(raisePolicy);
            raisePolicy = raisePolicyBuilder.AddCreditCardPayment(raisePolicy);

            var converter = GetServiceInstance<IRaisePolicyConverter>();
            var result = converter.From(raisePolicy);

            var fileAttachment = result.PolicyOrder.FileAttachment.First();

            Assert.That(fileAttachment.AttachmentDocument.DocumentURI, Is.EqualTo(raisePolicy.DocumentUrl));
        }


        private void AssertOptionAssignedIdentifier(LifeCoverageOption_Type[] options,
            string description, string value)
        {
            var option =
                options.FirstOrDefault(
                    x =>
                        x.AssignedIdentifier != null &&
                        x.AssignedIdentifier.First().IdentifierDescription == description);
            Assert.That(option, Is.Not.Null);
            Assert.That(option.AssignedIdentifier, Is.Not.Null);
            Assert.That(option.AssignedIdentifier.First().Id, Is.EqualTo(value));
        }

        private void AssertOptionAdded(LifeCoverageOption_Type[] options, string option)
        {
            var opt = options.FirstOrDefault(
                x => x.Description == option);
            Assert.That(opt, Is.Not.Null);
        }

        private void AssertOptionAddedAndValue(LifeCoverageOption_Type[] options, string option, string value)
        {
            var opt = options.FirstOrDefault(
                x => x.Description == option);
            Assert.That(opt, Is.Not.Null);
            Assert.That(opt.OptionCode.Value.Name, Is.EqualTo(value));

        }
    }

    public class RaisePolicyBuilder
    {
        public ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy AddCreditCardPayment(ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy baseRaisePolicy)
        {
            baseRaisePolicy.Payment = new RaisePolicyPayment()
            {
                PaymentType = PaymentType.CreditCard,
                CreditCard = new RaisePolicyCreditCard()
                {
                    CardNumber = "456411*******333",
                    CardType = CreditCardType.Visa,
                    NameOnCard = "Bob's Card",
                    ExpiryMonth = "11",
                    ExpiryYear = "18",
                    Token = "f549d7c7b27a4442af4cc9d6fd04bd7b"
                }
            };

            return baseRaisePolicy;
        }

        public ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy AddLifePlan(
            ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy baseRaisePolicy)
        {
            var planId = baseRaisePolicy.PrimaryRisk.Plans.Count + 1;

            baseRaisePolicy.PrimaryRisk.Plans.Add(new RaisePolicyPlan()
            {
                Code = ProductPlanConstants.LifePlanCode,
                Id = planId,
                Covers = new List<RaisePolicyCover>()
                {
                    new RaisePolicyCover()
                    {
                        Code = ProductCoverConstants.LifeAccidentCover,
                        PlanId = planId,
                        CoverAmount = 1500000,
                        Selected = true
                    },
                    new RaisePolicyCover()
                    {
                        Code = ProductCoverConstants.LifeIllnessCover,
                        PlanId = planId,
                        CoverAmount = 1500000,
                        Selected = true
                    },
                    new RaisePolicyCover()
                    {
                        Code = ProductCoverConstants.LifeAdventureSportsCover,
                        PlanId = planId,
                        CoverAmount = 1500000,
                        Selected = true
                    }
                },
                Options = new List<RaisePolicyOption>()
                {
                    new RaisePolicyOption() {Code = "PR", PlanId = planId, RiskId =baseRaisePolicy.PrimaryRisk.Id, Selected = true},
                },
                LinkedToCpi = true,
                Premium =  1000m,
                MultiPlanDiscountFactor = .9m
            });

            return baseRaisePolicy;
        }

        public ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy AddCiPlan(
           ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy baseRaisePolicy)
        {
            var planId = baseRaisePolicy.PrimaryRisk.Plans.Count + 1;

            baseRaisePolicy.PrimaryRisk.Plans.Add(new RaisePolicyPlan()
            {
                Code = ProductPlanConstants.CriticalIllnessPlanCode,
                Id = planId,
                Covers = new List<RaisePolicyCover>()
                {
                    new RaisePolicyCover()
                    {
                        Code = ProductCoverConstants.CiSeriousInjuryCover,
                        PlanId = planId,
                        CoverAmount = 250000,
                        Selected = true
                    },
                    new RaisePolicyCover()
                    {
                        Code = ProductCoverConstants.CiSeriousIllnessCover,
                        PlanId = planId,
                        CoverAmount = 250000,
                        Selected = false
                    },
                    new RaisePolicyCover()
                    {
                        Code = ProductCoverConstants.CiCancerCover,
                        PlanId = planId,
                        CoverAmount = 250000,
                        Selected = false
                    }
                },
                Options = new List<RaisePolicyOption>()
                {
                    new RaisePolicyOption() {Code = "PR", PlanId = planId, RiskId =baseRaisePolicy.PrimaryRisk.Id, Selected = true},
                },
                LinkedToCpi = true,
                Premium = 1000m
            });

            return baseRaisePolicy;
        }

        public ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy AddIpPlan(
           ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy baseRaisePolicy)
        {
            var planId = baseRaisePolicy.PrimaryRisk.Plans.Count + 1;

            baseRaisePolicy.PrimaryRisk.Plans.Add(new RaisePolicyPlan()
            {
                Code = ProductPlanConstants.IncomeProtectionPlanCode,
                Id = planId,
                Covers = new List<RaisePolicyCover>()
                {
                    new RaisePolicyCover()
                    {
                        Code = ProductCoverConstants.IpAccidentCover,
                        PlanId = planId,
                        CoverAmount = 10000,
                        Selected = true
                    },
                    new RaisePolicyCover()
                    {
                        Code = ProductCoverConstants.IpIllnessCover,
                        PlanId = planId,
                        CoverAmount = 10000,
                        Selected = true
                    },
                    new RaisePolicyCover()
                    {
                        Code = ProductCoverConstants.IpAdventureSportsCover,
                        PlanId = planId,
                        CoverAmount = 10000,
                        Selected = true
                    }
                },
                WaitingPeriod = 4,
                BenefitPeriod = 1,
                LinkedToCpi =false,
                Options = new List<RaisePolicyOption>()
                {
                    new RaisePolicyOption() {Code = "DOA", PlanId = planId, RiskId =baseRaisePolicy.PrimaryRisk.Id, Selected = true},
                }
            });

            return baseRaisePolicy;
        }

        public ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy AddRiderWithAllOptions(
            ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy baseRaisePolicy)
        {
            var planId = baseRaisePolicy.PrimaryRisk.Plans.Count + 1;

            baseRaisePolicy.PrimaryRisk.Plans.Add(new RaisePolicyPlan()
            {
                Code = "TPDDTH",
                Id = planId,
                Covers = new List<RaisePolicyCover>()
                {
                    new RaisePolicyCover()
                    {
                        Code = "TPDDTHAC",
                        PlanId = planId,
                        CoverAmount = 1500000,
                        Selected = true
                    }
                },
                Options = new List<RaisePolicyOption>()
                {
                    new RaisePolicyOption() {Code = "PR", PlanId = planId, RiskId =baseRaisePolicy.PrimaryRisk.Id, Selected = true},
                    new RaisePolicyOption() {Code = "DOA", PlanId = planId, RiskId =baseRaisePolicy.PrimaryRisk.Id, Selected = true},
                    new RaisePolicyOption() {Code = "TPDDTHDBB", PlanId = planId, RiskId =baseRaisePolicy.PrimaryRisk.Id, Selected = true},
                },
                OccupationDefinition = OccupationDefinition.OwnOccupation
            });

            return baseRaisePolicy;
        }

        public ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy AddLastCompletedReferral(
            ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy baseRaisePolicy, DateTime lastCompletedreferral)
        {
            baseRaisePolicy.LastCompletedReferralDateTime = lastCompletedreferral;
            return baseRaisePolicy;
        }

        public ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy GetBaseRaisePolicy()
        {
            var raisePolicyObject = new ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy
            {
                ReadyToSubmitDateTime = DateTime.Now,
                PremiumFrequency = PremiumFrequency.Monthly,
                QuoteReference = "M123456789",
                DocumentUrl = "https://local-talsalesportal.delivery.lan/Policy/Edit/M123456789",
                Owner = new RaisePolicyOwner
                {
                    Address = "12 Happy St",
                    Suburb = "Happy Valley",
                    Country = Country.Australia,
                    State = State.VIC,
                    Postcode = "3031",

                    Surname = "Test",
                    FirstName = "Jimmy",
                    Title = Title.Mr,

                    MobileNumber = "0400000000",
                    HomeNumber = "0200000000",
                    EmailAddress = "jim.barnes@hotmail.com",

                    OwnerType = PolicyOwnerType.Ordinary
                },
                PrimaryRisk = new RaisePolicyRisk
                {
                    DateOfBirth = DateTime.Now.AddYears(-30),
                    Gender = Gender.Male,
                    SmokerStatus = SmokerStatus.No,
                    PartyId = 1,
                    PolicyId = 1,
                    AnnualIncome = 50000,
                    Address = "12 Happy St",
                    Suburb = "Happy Valley",
                    Country = Country.Australia,
                    State = State.VIC,
                    Postcode = "3031",
                    Surname = "Test",
                    FirstName = "Jimmy",
                    Title = Title.Mr,
                    MobileNumber = "0400000000",
                    HomeNumber = "0200000000",
                    EmailAddress = "jim.barnes@hotmail.com",
                    Id = 1,
                    InterviewId = "1000",
                    PasCode = "PASCODE",
                    Plans = new List<RaisePolicyPlan>(),
                    Beneficiaries = new List<RaisePolicyBeneficiary>()
                    {
                        new RaisePolicyBeneficiary()
                        {
                            DateOfBirth = DateTime.Now.AddYears(-30),
                            Gender = Gender.Female,
                            Address = "121 Joy St",
                            Suburb = "Joy Valley",
                            Country = Country.Australia,
                            State = State.VIC,
                            Postcode = "3032",
                            Surname = "Test",
                            FirstName = "Jill",
                            Title = Title.Mrs,
                            PhoneNumber = "0400000000",
                            EmailAddress = "jill.smith@hotmail.com",
                            Id = 2,
                            Share = 100,
                            BeneficiaryRelationshipId = 1,  //"Spouse"
                            RiskId = 1
                        }
                    }
                },
                Id = 123456
            };

            return raisePolicyObject;
        }

        public ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy AddPerMilleLoadingToLifePlanAccidentCover(ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy raisePolicy, decimal loadingAmount)
        {
            var accidentCover = raisePolicy.PrimaryRisk.Plans.First(x => x.Code == "DTH").Covers.First(x => x.Code == "DTHAC");
            accidentCover.Loadings.Add(new RaisePolicyCoverLoading {CoverId = accidentCover.Id, Loading = loadingAmount, LoadingType = LoadingType.PerMille});
            return raisePolicy;
        }

        public ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy AddPercentageLoadingToLifePlanAccidentCover(ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy raisePolicy, decimal loadingAmount)
        {
            var accidentCover = raisePolicy.PrimaryRisk.Plans.First(x => x.Code == "DTH").Covers.First(x => x.Code == "DTHAC");
            accidentCover.Loadings.Add(new RaisePolicyCoverLoading { CoverId = accidentCover.Id, Loading = loadingAmount, LoadingType = LoadingType.Variable });
            return raisePolicy;
        }

        public ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy AddExclusionToLifePlanAccidentCover(ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy raisePolicy)
        {
            var accidentCover = raisePolicy.PrimaryRisk.Plans.First(x => x.Code == "DTH").Covers.First(x => x.Code == "DTHAC");
            accidentCover.Exclusions.Add(new RaisePolicyCoverExclusion());
            return raisePolicy;
        }

        public ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy TurnLifeAccidentCoverBlockOff(ServiceLayer.Policy.RaisePolicy.Models.RaisePolicy raisePolicy)
        {
            var accidentCover = raisePolicy.PrimaryRisk.Plans.First(x => x.Code == "DTH").Covers.First(x => x.Code == "DTHAC");
            accidentCover.Selected = false;
            return raisePolicy;
        }
    }
}

