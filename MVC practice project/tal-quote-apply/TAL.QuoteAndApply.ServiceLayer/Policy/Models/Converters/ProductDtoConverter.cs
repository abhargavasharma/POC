using System.Collections.Generic;
using System.Linq;
using System.Monads;
using TAL.QuoteAndApply.DataModel.Payment;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Product.Models.Definition;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models.Conveters;
using TAL.QuoteAndApply.ServiceLayer.Product;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters
{
    public interface IProductDtoConverter
    {
        ProductDetailsResult CreateFrom(ProductDefinition productDefinition);
        PlanResponse CreateFrom(PlanDefinition plan);
    }

    public class ProductDtoConverter : IProductDtoConverter
    {
        public PlanDto CreateFrom(PlanStateParam planOptionsParam)
        {
            return new PlanDto
            {
                CoverAmount = planOptionsParam.CoverAmount,
                PolicyId = planOptionsParam.PolicyId,
                RiskId = planOptionsParam.RiskId
            };
        }

        public ProductDetailsResult CreateFrom(ProductDefinition productDefinition)
        {
            var productDetails = new ProductDetailsResult
            {
                Name = productDefinition.Name,
                Code = productDefinition.Code,
                MinimumAnnualIncomeDollars = productDefinition.MinimumAnnualIncomeDollars,
                PremiumTypes = productDefinition.PremiumTypes.Select(type => new PremiumTypeResponse
                {
                    PremiumType = type.PremiumType,
                    Name = type.Name,
                    MaximumEntryAgeNextBirthday = type.MaximumEntryAgeNextBirthday,
                    IsUserSelectable = type.IsUserSelectable
                }),
                Plans = productDefinition.Plans.Select(CreateFrom),
                MaximumNumberOfBeneficiaries = productDefinition.MaximumNumberOfBeneficiaries,
				IsQuoteSaveLoadEnabled = productDefinition.IsQuoteSaveLoadEnabled,
                SaveGatePosition = productDefinition.SaveGatePosition.ToString(),
                AvailableOwnerTypes =
                    new List<PolicyOwnerTypeParam>
                    {
                        new PolicyOwnerTypeParam {DisplayName = "Ordinary", Value = PolicyOwnerType.Ordinary.ToString()}
                    }
            };

            if (productDefinition.PaymentOptions.Any(po => po.PaymentType == PaymentType.CreditCard))
            {
                var paymentOption = productDefinition.PaymentOptions.First(po => po.PaymentType == PaymentType.CreditCard) as CreditCardPaymentDefinition;
                productDetails.AvailableCreditCardTypes = paymentOption.AvailableCreditCardTypes.Select(t => new CreditCardTypeConverter().From(t));
            }

            if (productDefinition.PaymentOptions.Any(po => po.PaymentType == PaymentType.DirectDebit))
            {
                productDetails.IsDirectDebitAvailable = true;
            }

            if (productDefinition.PaymentOptions.Any(po => po.PaymentType == PaymentType.SuperAnnuation))
            {
                productDetails.IsSuperannuationAvailable = true;
                productDetails.AvailableOwnerTypes.Add(new PolicyOwnerTypeParam { DisplayName = "SuperPay", Value = PolicyOwnerType.SuperannuationFund.ToString() });
            }

            if (productDefinition.PaymentOptions.Any(po => po.PaymentType == PaymentType.SelfManagedSuperFund))
            {
                productDetails.IsSmsfAvailable = true;
                productDetails.AvailableOwnerTypes.Add(new PolicyOwnerTypeParam {DisplayName = "SMSF", Value = PolicyOwnerType.SelfManagedSuperFund.ToString()});
            }

            return productDetails;
        }

        public PlanResponse CreateFrom(PlanDefinition plan)
        {
            return new PlanResponse
            {
                Name = plan.Name,
                ShortName = plan.ShortName,
                Code = plan.Code,
                RelatedPlanCode = plan.RelatedPlanCode,
                PlanType = "Cover",
                Covers = plan.Covers.IsNotNull() ? plan.Covers.Select(cover => new CoverResponse
                {
                    Code = cover.Code,
                    Name = cover.Name,
                    CoverFor = cover.CoverFor,
                    UnderwritingBenefitCode = cover.UnderwritingBenefitCode
                }) : new List<CoverResponse>(),
                Options = plan.Options.IsNotNull() ? plan.Options.Select(option => new OptionResponse
                {
                    Code = option.Code,
                    Name = option.Name,
                    Selected = option.InitiallySelected
                }) : new List<OptionResponse>(),
                Riders = plan.Riders.IsNotNull() ? plan.Riders.Select(rider => new PlanResponse
                {
                    ShortName = rider.ShortName,
                    Code = rider.Code,
                    RelatedPlanCode = rider.RelatedPlanCode,
                    Covers = rider.Covers.IsNotNull() ? rider.Covers.Select(riderCover => new CoverResponse
                    {
                        Code = riderCover.Code,
                        Name = riderCover.Name,
                        CoverFor = riderCover.CoverFor,
                        UnderwritingBenefitCode = riderCover.UnderwritingBenefitCode
                    }) : new List<CoverResponse>(),
                    Name = rider.Name,
                    Options = rider.Options.IsNotNull() ? rider.Options.Select(riderOption => new OptionResponse
                    {
                        Code = riderOption.Code,
                        Name = riderOption.Name,
                        Selected = riderOption.InitiallySelected
                    }) : new List<OptionResponse>(),
                    Variables = rider.Variables.IsNotNull() ? rider.Variables.Select(variable => new VariableResponse
                    {
                        Code = variable.Code,
                        Name = variable.Name
                    }) : new List<VariableResponse>()
                }) : new List<PlanResponse>(),
                Variables = plan.Variables.IsNotNull() ? plan.Variables.Select(variable => new VariableResponse
                { 
                    Code = variable.Code,
                    Name = variable.Name,
                    Options = variable.Options.Select(o => new VariableOptionResponse(o.Name, o.Value)).ToList(),
                    AllowEditingBy = variable.AllowEditingBy
                }) : new List<VariableResponse>()
            };
        }
    }
}
