using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TAL.QuoteAndApply.ServiceLayer.RulesProxy;

namespace TAL.QuoteAndApply.Web.Shared.Attributes.Validation
{

    public class BoolIsTrueAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            return (bool)value;
        }
    }
}
