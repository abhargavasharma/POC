using System;
using System.Linq;
using System.Web.Mvc;
using TAL.QuoteAndApply.Infrastructure.Extensions;
using TAL.QuoteAndApply.Infrastructure.Logging;

namespace TAL.QuoteAndApply.Customer.Web.Models.Binders
{
    public class JsonModelBinder : IModelBinder
    {
        private readonly ILoggingService _loggingService;

        public JsonModelBinder()
        {
            _loggingService =  DependencyResolver.Current.GetService<ILoggingService>();
        }

        public JsonModelBinder(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            try
            {
                Type modelType = bindingContext.ModelType;
                string modelName = bindingContext.ModelName;

                ValueProviderResult res = bindingContext.ValueProvider.GetValue(modelName);

                if (res != null && res.RawValue is string[])
                {
                    var vals = ((string[]) res.RawValue);

                    if (vals.Any())
                    {
                        var rawValue = vals[0];


                        return rawValue.FromJson(modelType);
                    }
                }
            }
            catch(Exception err)
            {
                _loggingService.Error("Entry payload failed to parse", err);
            }
        
            return null;
        }
    }
}