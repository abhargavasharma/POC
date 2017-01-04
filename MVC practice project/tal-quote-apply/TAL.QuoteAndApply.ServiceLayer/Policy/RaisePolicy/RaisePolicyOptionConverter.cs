using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy
{
    public interface IRaisePolicyOptionConverter
    {
        RaisePolicyOption From(IOption option);
    }

    public class RaisePolicyOptionConverter : IRaisePolicyOptionConverter
    {
        public RaisePolicyOption From(IOption option)
        {
            return new RaisePolicyOption
            {
                Code = option.Code,
                Selected = option.Selected,
                RiskId = option.RiskId,
                PlanId = option.PlanId
            };
        }
    }
}
