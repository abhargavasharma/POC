using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.Models
{
    public class OptionsParam
    {
        public string Code { get; }
        public bool? Selected { get; set; }

        public OptionsParam(string code, bool? selected)
        {
            Code = code;
            Selected = selected;
        }
    }
}
