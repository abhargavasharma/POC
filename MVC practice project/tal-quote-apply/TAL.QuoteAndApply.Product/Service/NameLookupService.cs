using System;
using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Product.Definition;

namespace TAL.QuoteAndApply.Product.Service
{
    public interface INameLookupService
    {
        string GetCoverNames(IEnumerable<string> features, string planCode, string brandKey);
        string GetOptionNames(IEnumerable<string> coverCodes, string planCode, string brandKey);
        string GetPlanNames(IEnumerable<string> planCodes, string planCode, string brandKey);
        string GetPlanName(string planCode, string brandKey);
        string GetPlanShortName(string planCode, string brandKey);
        string GetCoverName(string planCode, string coverCode, string brandKey);
        string GetOptionName(string planCode, string optionCode, string brandKey);
        string GetVariableName(string planCode, string variableCode, string brandKey);
        string GetVariableOptionName(string planCode, string variableCode, object variableValue, string brandKey);
        string GetCoverNamesForRider(IEnumerable<string> features, string planCode, string brandKey);
    }

    public class NameLookupService : INameLookupService
    {
        private readonly IPlanDefinitionProvider _planDefinitionProvider;
        private readonly IProductDefinitionBuilder _marketProductDefinitionBuilder;

        public NameLookupService(IPlanDefinitionProvider planDefinitionProvider, IProductDefinitionBuilder marketProductDefinitionBuilder)
        {
            _planDefinitionProvider = planDefinitionProvider;
            _marketProductDefinitionBuilder = marketProductDefinitionBuilder;
        }

        public string GetCoverName(string planCode, string coverCode, string brandKey)
        {
            var planDefinition = _planDefinitionProvider.GetPlanByCode(planCode, brandKey);

            if (planDefinition.Covers != null)
            {
                coverCode = planDefinition.Covers.First(cov => cov.Code == coverCode).Name;
            }

            return coverCode;
        }

        public string GetVariableName(string planCode, string variableCode, string brandKey)
        {
            var planDefinition = _planDefinitionProvider.GetPlanByCode(planCode, brandKey);

            if (planDefinition.Variables != null)
            {
                variableCode = planDefinition.Variables.First(vari => vari.Code == variableCode).Name;
            }

            return variableCode;
        }

        public string GetVariableOptionName(string planCode, string variableCode, object variableValue, string brandKey)
        {
            var planDefinition = _planDefinitionProvider.GetPlanByCode(planCode, brandKey);

            if (planDefinition.Variables != null)
            {
                var variable = planDefinition.Variables.First(vari => vari.Code == variableCode);
                var variableOption = variable.Options.FirstOrDefault(o => o.Value.Equals(variableValue));
                return variableOption?.Name;
            }

            return null;
        }

        public string GetCoverNamesForRider(IEnumerable<string> coverCodes, string planCode, string brandKey)
        {
            var planDefinition = _planDefinitionProvider.GetPlanByCode(planCode, brandKey);
            var returnListOfCoverCodes = (from cover 
                                          in planDefinition.Covers
                                          where coverCodes.Contains(cover.Code)
                                          select string.Format("{0} for Life", cover.Name)).ToList();

            if (planDefinition.Riders != null)
            {
                foreach (var rider in planDefinition.Riders)
                {
                    foreach (var riderCover in rider.Covers.Where(c => coverCodes.Contains(c.Code)))
                    {
                        returnListOfCoverCodes.Add(string.Format("{0} for Life", riderCover.Name));
                    }
                }
            }

            //todo: This makes Chris feel dirty. Horrible
            var returnString = string.Join(" and ", returnListOfCoverCodes);
            if (returnString.IndexOf("adventure sports cover for life and accident cover for life", StringComparison.OrdinalIgnoreCase) != -1)
                returnString =
                    "Adventure Sports and Accident Cover for Life";
            return returnString;
        }

        public string GetOptionName(string planCode, string optionCode, string brandKey)
        {
            var planDefinition = _planDefinitionProvider.GetPlanByCode(planCode, brandKey);

            if (planDefinition.Options != null)
            {
                optionCode = planDefinition.Options.First(opt => opt.Code == optionCode).Name;
            }
            return optionCode;
        }

        public string GetCoverNames(IEnumerable<string> coverCodes, string planCode, string brandKey)
        {
            var planDefinition = _planDefinitionProvider.GetPlanByCode(planCode, brandKey);
            var returnListOfCoverCodes = new List<string>();
            foreach (var cover in planDefinition.Covers)
            {
                if (coverCodes.Contains(cover.Code))
                {
                    returnListOfCoverCodes.Add(cover.Name);
                }
            }
            if (planDefinition.Riders != null)
            {
                foreach (var rider in planDefinition.Riders)
                {
                    foreach (var riderCover in rider.Covers)
                    {
                        if (coverCodes.Contains(riderCover.Code))
                        {
                            returnListOfCoverCodes.Add(riderCover.Name);
                        }
                    }
                }
            }
            
            var returnString = string.Join(", ", returnListOfCoverCodes);
            return returnString;
        }

        public string GetOptionNames(IEnumerable<string> coverCodes, string planCode, string brandKey)
        {
            var planDefinition = _planDefinitionProvider.GetPlanByCode(planCode, brandKey);
            var returnListOfOptionCodes = new List<string>();
            foreach (var option in planDefinition.Options)
            {
                if (coverCodes.Contains(option.Code))
                {
                    returnListOfOptionCodes.Add(option.Name);
                }
            }
            var returnString = string.Join(", ", returnListOfOptionCodes);
            return returnString;
        }

        public string GetPlanNames(IEnumerable<string> planCodes, string planCode, string brandKey)
        {
            var productDefinition = _marketProductDefinitionBuilder.BuildProductDefinition(brandKey);
            var returnListOfPlanCodes = new List<string>();
            foreach (var plan in productDefinition.Plans)
            {
                if (planCodes.Contains(plan.Code))
                {
                    returnListOfPlanCodes.Add(plan.Name);
                }
            }
            var returnString = string.Join(", ", returnListOfPlanCodes);
            return returnString;
        }

        public string GetPlanName(string planCode, string brandKey)
        {
            var planDefinition = _planDefinitionProvider.GetPlanByCode(planCode, brandKey);
            return planDefinition.Name;
        }

        public string GetPlanShortName(string planCode, string brandKey)
        {
            var planDefinition = _planDefinitionProvider.GetPlanByCode(planCode, brandKey);
            return planDefinition.ShortName;
        }
    }
}