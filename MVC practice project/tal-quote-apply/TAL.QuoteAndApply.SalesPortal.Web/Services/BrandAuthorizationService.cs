using System.Linq;
using TAL.QuoteAndApply.DataModel.User;
using TAL.QuoteAndApply.SalesPortal.Web.User;
using TAL.QuoteAndApply.ServiceLayer.Policy.Models;
using TAL.QuoteAndApply.ServiceLayer.Product;

namespace TAL.QuoteAndApply.SalesPortal.Web.Services
{
    public interface IBrandAuthorizationService
    {
        bool CanAccess(PolicyOverviewResult policy);
        bool CanAccess(string quoteReferenceNumber);
        bool CanAccess(int riskId);
    }

    public class BrandAuthorizationService: IBrandAuthorizationService
    {
        private readonly ICurrentProductBrandProvider _currentBrandProvider;
        private readonly IProductBrandProvider _productBrandProvider;
        private readonly ISalesPortalSessionContext _salesPortalSessionContext;

        public BrandAuthorizationService(ICurrentProductBrandProvider currentProductBrandProvider, 
            IProductBrandProvider productBrandProvider, 
            ISalesPortalSessionContext salesPortalSessionContext)
        {
            _currentBrandProvider = currentProductBrandProvider;
            _productBrandProvider = productBrandProvider;
            _salesPortalSessionContext = salesPortalSessionContext;
        }

        public bool CanAccess(PolicyOverviewResult policy)
        {
            return
                _salesPortalSessionContext.SalesPortalSession.Roles.Any(r => r == Role.Underwriter) ||
                _currentBrandProvider.GetCurrent().BrandCode == policy.Brand;
        }

        public bool CanAccess(string quoteReferenceNumber)
        {
            return
                _salesPortalSessionContext.SalesPortalSession.Roles.Any(r => r == Role.Underwriter) ||
                _currentBrandProvider.GetCurrent().BrandCode == _productBrandProvider.GetBrandKeyForQuoteReferenceNumber(quoteReferenceNumber);
        }

        public bool CanAccess(int riskId)
        {
            return
                _salesPortalSessionContext.SalesPortalSession.Roles.Any(r => r == Role.Underwriter) ||
                _currentBrandProvider.GetCurrent().BrandCode == _productBrandProvider.GetBrandKeyForRiskId(riskId);
        }
    }
}