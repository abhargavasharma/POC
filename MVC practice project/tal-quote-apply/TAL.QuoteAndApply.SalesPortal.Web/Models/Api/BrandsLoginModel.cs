using System.Collections.Generic;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.User;

namespace TAL.QuoteAndApply.SalesPortal.Web.Models.Api
{
    public class BrandsLoginModel
    {
        public IEnumerable<string> Brands { get; private set; }
        public bool IsUnderwriter { get; private set; }
        public BrandsLoginModel(IEnumerable<string> brands, bool isUnderwriter)
        {
            Brands = brands;
            IsUnderwriter = isUnderwriter;
        }
    }
}