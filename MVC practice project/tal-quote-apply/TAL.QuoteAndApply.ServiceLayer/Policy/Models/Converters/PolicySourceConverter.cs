using TAL.QuoteAndApply.ServiceLayer.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.PlanSelection;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models.Converters
{
    public interface IPolicySourceConverter
    {
        PlanSelectionType From(PolicySource policySource);
        DataModel.Policy.PolicySource ToDataModel(PolicySource policySource);
    }

    public class PolicySourceConverter : IPolicySourceConverter
    {
        public PlanSelectionType From(PolicySource policySource)
        {
            switch (policySource)
            {
                case PolicySource.CustomerPortalBuildMyOwn:
                case PolicySource.SalesPortal:
                case PolicySource.CustomerPortalHelpMeChoose:
                    return PlanSelectionType.LetMePlayDefault;
                case PolicySource.Unknown:
                    return PlanSelectionType.NotSet;
                default:
                    return PlanSelectionType.NotSet;
            }
        }

        public DataModel.Policy.PolicySource ToDataModel(PolicySource policySource)
        {
            switch (policySource)
            {
                case PolicySource.CustomerPortalBuildMyOwn:
                    return DataModel.Policy.PolicySource.CustomerPortalBuildMyOwn;
                case PolicySource.CustomerPortalHelpMeChoose:
                    return DataModel.Policy.PolicySource.CustomerPortalHelpMeChoose;
                case PolicySource.SalesPortal:
                    return DataModel.Policy.PolicySource.SalesPortal;
                default:
                    return DataModel.Policy.PolicySource.Unknown;
            }
        }
    }
}