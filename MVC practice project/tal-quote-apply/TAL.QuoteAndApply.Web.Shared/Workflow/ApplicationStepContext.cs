using System.Monads;
using System.Web;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;

namespace TAL.QuoteAndApply.Web.Shared.Workflow
{
    public class ApplicationStepContext
    {
        public ApplicationStepContext(PolicyOverviewResult application, HttpContextBase httpContext)
        {
            Application = application;
            HttpContext = httpContext;
        }

        public readonly PolicyOverviewResult Application;
        public readonly HttpContextBase HttpContext;

        public int NumberOfRisks
        {
            get
            {
                return Application
                    .With(app => app.Risks)
                    .Return(r => r.Count, 0);
            }
        }
    }
}