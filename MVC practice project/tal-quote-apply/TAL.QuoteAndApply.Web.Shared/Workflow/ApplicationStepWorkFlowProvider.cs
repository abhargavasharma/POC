using System;
using System.Collections.Generic;
using TAL.QuoteAndApply.Infrastructure.Cache;

namespace TAL.QuoteAndApply.Web.Shared.Workflow
{
    public class ApplicationStepWorkFlowProvider : IApplicationStepWorkFlowProvider
    {
        private readonly ICachingWrapper _cachingWrapper;

        public ApplicationStepWorkFlowProvider(ICachingWrapper cachingWrapper)
        {
            _cachingWrapper = cachingWrapper;
        }

        private IDictionary<string, IApplicationStepResolver> ApplicationStepResolvers
        {
            get
            {
                return _cachingWrapper.GetOrAddCacheItemRequestScope(GetType(), "ApplicationStepWorkFlowProvider", BuildDictionary);
            }
        }

        public IDictionary<string, IApplicationStepResolver> BuildDictionary()
        {
            var resolvers = new Dictionary<string, IApplicationStepResolver>(StringComparer.InvariantCultureIgnoreCase)
            {
                {ProductCodeConstants.ProductCode, ApplicationStepWorkFlowBuilder.BuildQuoteAndApplyWorkflow()},
            };
            return resolvers;
        }


        public IApplicationStepResolver GetForProduct(string productCode)
        {
            if (ApplicationStepResolvers.ContainsKey(productCode))
            {
                return ApplicationStepResolvers[productCode];
            }
            throw new Exception(string.Format("Product Code '{0}' has not been registered", productCode));
        }
    }
}