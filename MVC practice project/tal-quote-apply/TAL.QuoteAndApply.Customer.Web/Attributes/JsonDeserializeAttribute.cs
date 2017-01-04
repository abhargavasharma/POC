using System.Web.Mvc;
using TAL.QuoteAndApply.Customer.Web.Controllers.View;
using TAL.QuoteAndApply.Customer.Web.Models.Binders;

namespace TAL.QuoteAndApply.Customer.Web.Attributes
{
    public class JsonDeserializeAttribute : CustomModelBinderAttribute
    {
        public override IModelBinder GetBinder()
        {
            return new JsonModelBinder();
        }
    }
}