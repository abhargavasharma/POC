using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.DataModel.Payment;

namespace TAL.QuoteAndApply.Payment.Service.SuperFund
{
    public interface ISuperAnnuationFundProvider
    {
        IEnumerable<ISuperAnnuationFund> GetSuperFundsByName(string name);
        IEnumerable<ISuperAnnuationFund> GetSuperFundsByNameAndProduct(string name, string product);
        IEnumerable<ISuperAnnuationFund> GetSuperFundsByABN(string abn);
        IEnumerable<ISuperAnnuationFund> GetSuperFundsByUSI(string usi);
        IEnumerable<ISuperAnnuationFund> GetSuperFundsByABNAndProduct(string abn, string product);
    }

    public class SuperAnnuationFundProvider : ISuperAnnuationFundProvider
    {
        private readonly IHttpSuperFundLookupService _superFundLookupService;
        private readonly ISuperFundConverter _superFundConverter;

        public SuperAnnuationFundProvider(IHttpSuperFundLookupService superFundLookupService,
            ISuperFundConverter superFundConverter)
        {
            _superFundLookupService = superFundLookupService;
            _superFundConverter = superFundConverter;
        }

        public IEnumerable<ISuperAnnuationFund> GetSuperFundsByName(string name)
        {
            var searchResult = _superFundLookupService.Search(new SuperFundLookupParams() {Organization = name});
            return searchResult?.InvestmentInquiry.Select(ii => _superFundConverter.From(ii)) ?? new List<Models.SuperFund>();
        }

        public IEnumerable<ISuperAnnuationFund> GetSuperFundsByNameAndProduct(string name, string product)
        {
            var searchResult = _superFundLookupService.Search(new SuperFundLookupParams() { Organization = name, Product = product});
            return searchResult.InvestmentInquiry.Select(ii => _superFundConverter.From(ii));
        }

        public IEnumerable<ISuperAnnuationFund> GetSuperFundsByABN(string abn)
        {
            var searchResult = _superFundLookupService.Search(new SuperFundLookupParams() { ABN = abn });
            return searchResult.InvestmentInquiry.Select(ii => _superFundConverter.From(ii));
        }

        public IEnumerable<ISuperAnnuationFund> GetSuperFundsByABNAndProduct(string abn, string product)
        {
            var searchResult = _superFundLookupService.Search(new SuperFundLookupParams() { ABN = abn, Product = product});
            return searchResult.InvestmentInquiry.Select(ii => _superFundConverter.From(ii));
        }

        public IEnumerable<ISuperAnnuationFund> GetSuperFundsByUSI(string usi)
        {
            var searchResult = _superFundLookupService.Search(new SuperFundLookupParams() { USI = usi });
            return searchResult.InvestmentInquiry.Select(ii => _superFundConverter.From(ii));
        }
    }
}
