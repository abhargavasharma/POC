using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.Policy.Data;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models
{
    public class RaisePolicyOption : IOption
    {
        public int Id { get; }
        public int RiskId { get; set; }
        public string Code { get; set; }
        public bool? Selected { get; set; }
        public int PlanId { get; set; }
    }
}
