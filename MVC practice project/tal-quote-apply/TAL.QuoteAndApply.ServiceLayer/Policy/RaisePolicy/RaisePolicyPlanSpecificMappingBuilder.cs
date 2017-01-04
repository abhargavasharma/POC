using System.Collections.Generic;
using TAL.QuoteAndApply.Product.Definition;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy
{
    public interface IRaisePolicyPlanSpecificMappingBuilder
    {
        IDictionary<string, RaisePolicyPlanSpecificMapping> Build();
    }

    public class RaisePolicyPlanSpecificMappingBuilder : IRaisePolicyPlanSpecificMappingBuilder
    {
        public IDictionary<string, RaisePolicyPlanSpecificMapping> Build()
        {
            return new Dictionary<string, RaisePolicyPlanSpecificMapping>
            {
                { ProductPlanConstants.LifePlanCode , new RaisePolicyPlanSpecificMapping(ProductPlanConstants.LifePlanCode, "Death",
                covers: new Dictionary<string, RaisePolicyCoverSpecificMapping>
                {
                    { ProductCoverConstants.LifeAccidentCover, new RaisePolicyCoverSpecificMapping(RaisePolicyCoverType.AccidentOrSeriousInjury) },
                    { ProductCoverConstants.LifeIllnessCover, new RaisePolicyCoverSpecificMapping(RaisePolicyCoverType.Illness) },
                    { ProductCoverConstants.LifeAdventureSportsCover, new RaisePolicyCoverSpecificMapping(RaisePolicyCoverType.SportsOrCancer) },
                }) },
                { ProductRiderConstants.PermanentDisabilityRiderCode, new RaisePolicyPlanSpecificMapping(ProductRiderConstants.PermanentDisabilityRiderCode, "TotalAndPermanentDisability",
                covers: new Dictionary<string, RaisePolicyCoverSpecificMapping>
                {
                    { ProductRiderCoverConstants.TpdRiderAccidentCover, new RaisePolicyCoverSpecificMapping(RaisePolicyCoverType.AccidentOrSeriousInjury) },
                    { ProductRiderCoverConstants.TpdRiderIllnessCover, new RaisePolicyCoverSpecificMapping(RaisePolicyCoverType.Illness) },
                    { ProductRiderCoverConstants.TpdRiderAdventureSportsCover, new RaisePolicyCoverSpecificMapping(RaisePolicyCoverType.SportsOrCancer) },
                }) },
                { ProductRiderConstants.CriticalIllnessRiderCode, new RaisePolicyPlanSpecificMapping(ProductRiderConstants.CriticalIllnessRiderCode, "CriticalIllness", 
                covers: new Dictionary<string, RaisePolicyCoverSpecificMapping>
                {
                    { ProductRiderCoverConstants.CiRiderSeriousInjuryCover, new RaisePolicyCoverSpecificMapping(RaisePolicyCoverType.AccidentOrSeriousInjury) },
                    { ProductRiderCoverConstants.CiRiderSeriousIllnessCover, new RaisePolicyCoverSpecificMapping(RaisePolicyCoverType.Illness) },
                    { ProductRiderCoverConstants.CiRiderCancerCover, new RaisePolicyCoverSpecificMapping(RaisePolicyCoverType.SportsOrCancer) },
                }) },
                { ProductPlanConstants.CriticalIllnessPlanCode, new RaisePolicyPlanSpecificMapping(ProductPlanConstants.CriticalIllnessPlanCode, "CriticalIllness",
                covers: new Dictionary<string, RaisePolicyCoverSpecificMapping>
                {
                    { ProductCoverConstants.CiSeriousInjuryCover, new RaisePolicyCoverSpecificMapping(RaisePolicyCoverType.AccidentOrSeriousInjury) },
                    { ProductCoverConstants.CiSeriousIllnessCover, new RaisePolicyCoverSpecificMapping(RaisePolicyCoverType.Illness) },
                    { ProductCoverConstants.CiCancerCover, new RaisePolicyCoverSpecificMapping(RaisePolicyCoverType.SportsOrCancer) },
                }) },
                { ProductPlanConstants.PermanentDisabilityPlanCode, new RaisePolicyPlanSpecificMapping(ProductPlanConstants.PermanentDisabilityPlanCode, "TotalAndPermanentDisability", 
                covers: new Dictionary<string, RaisePolicyCoverSpecificMapping>
                {
                    { ProductCoverConstants.TpdAccidentCover, new RaisePolicyCoverSpecificMapping(RaisePolicyCoverType.AccidentOrSeriousInjury) },
                    { ProductCoverConstants.TpdIllnessCover, new RaisePolicyCoverSpecificMapping(RaisePolicyCoverType.Illness) },
                    { ProductCoverConstants.TpdAdventureSportsCover, new RaisePolicyCoverSpecificMapping(RaisePolicyCoverType.SportsOrCancer) },
                }) },
                { ProductPlanConstants.IncomeProtectionPlanCode, new RaisePolicyPlanSpecificMapping(ProductPlanConstants.IncomeProtectionPlanCode, "IncomeProtection",
                covers: new Dictionary<string, RaisePolicyCoverSpecificMapping>
                {
                    { ProductCoverConstants.IpAccidentCover, new RaisePolicyCoverSpecificMapping(RaisePolicyCoverType.AccidentOrSeriousInjury) },
                    { ProductCoverConstants.IpIllnessCover, new RaisePolicyCoverSpecificMapping(RaisePolicyCoverType.Illness) },
                    { ProductCoverConstants.IpAdventureSportsCover, new RaisePolicyCoverSpecificMapping(RaisePolicyCoverType.SportsOrCancer) },
                }) },
            };
        }
    }
}