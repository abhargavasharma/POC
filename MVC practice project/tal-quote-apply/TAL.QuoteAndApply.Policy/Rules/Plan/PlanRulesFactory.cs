using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Policy.Models;

namespace TAL.QuoteAndApply.Policy.Rules.Plan
{
    public interface IPlanRulesFactory
    {
        IEnumerable<IOccupationDefinitionIsAllowedRule> GetOccupationDefinitionIsAllowedRule(IRisk risk);
    }

    public class PlanRulesFactory : IPlanRulesFactory
    {
        public IEnumerable<IOccupationDefinitionIsAllowedRule> GetOccupationDefinitionIsAllowedRule(IRisk risk)
        {
            yield return new OccupationDefinitionIsAllowedRule(risk);
        }
    }
}
