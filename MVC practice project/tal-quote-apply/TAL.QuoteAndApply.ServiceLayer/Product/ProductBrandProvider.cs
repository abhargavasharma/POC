using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service;

namespace TAL.QuoteAndApply.ServiceLayer.Product
{
    public interface IProductBrandProvider
    {
        string GetBrandKeyForRisk(IRisk risk);
        string GetBrandKeyForQuoteReferenceNumber(string quoteReference);
        string GetBrandKeyForRiskId(int riskId);
        string GetBrandKeyForCover(ICover cover);
        int GetBrandIdByKey(string brandKey);
        string GetBrandKeyByBrandId(int id);
        bool CheckIfBrandExists(string brandKey);
    }

    public class ProductBrandProvider: IProductBrandProvider
    {
        private readonly IPolicyService _policyService;
        private readonly IRiskService _riskService;
        private readonly IBrandDtoRepository _brandDtoRepository;

        public ProductBrandProvider(IPolicyService policyService, IRiskService riskService, IBrandDtoRepository brandDtoRepository)
        {
            _policyService = policyService;
            _riskService = riskService;
            _brandDtoRepository = brandDtoRepository;
        }

        public string GetBrandKeyForRisk(IRisk risk)
        {
            var policy = _policyService.Get(risk.PolicyId);
            return policy.BrandKey;
        }

        public string GetBrandKeyForRiskId(int riskId)
        {
            var risk = _riskService.GetRisk(riskId);
            return GetBrandKeyForRisk(risk);
        }

        public string GetBrandKeyForQuoteReferenceNumber(string quoteReference)
        {
            var policy = _policyService.GetByQuoteReferenceNumber(quoteReference);
            return policy.BrandKey;
        }

        public string GetBrandKeyForCover(ICover cover)
        {
            var policy = _policyService.Get(cover.PolicyId);
            return policy.BrandKey;
        }

        public int GetBrandIdByKey(string brandKey)
        {
            const int defaultBrand = 1;
            //if not brand then use TAL
            if (brandKey == null)
            {
                return defaultBrand;
            }
            return _brandDtoRepository.GetBrand(brandKey)?.Id ?? defaultBrand;
        }

        public bool CheckIfBrandExists(string brandKey)
        {
            return _brandDtoRepository.GetBrand(brandKey) != null;
        }

        public string GetBrandKeyByBrandId(int id)
        {
            return _brandDtoRepository.GetBrand(id)?.ProductKey;
        }
    }
}
