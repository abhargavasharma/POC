using System.Collections.Generic;
using System.Linq;
using TAL.QuoteAndApply.Payment.Service.SuperFund;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models.Conveters;

namespace TAL.QuoteAndApply.ServiceLayer.Payment
{
    public interface ISuperannuationFundSearch
    {
        IEnumerable<SuperannuationFundSearchResponse> GetSuperFundsByName(string name);
        IEnumerable<SuperannuationFundSearchResponse> GetSuperFundsByNameAndProduct(string name, string product);
        IEnumerable<SuperannuationFundSearchResponse> GetSuperFundsByABN(string abn);
        IEnumerable<SuperannuationFundSearchResponse> GetSuperFundsByUSI(string usi);
        IEnumerable<SuperannuationFundSearchResponse> GetSuperFundsByABNAndProduct(string abn, string product);
    }

    public class SuperannuationFundSearch : ISuperannuationFundSearch
    {
        private readonly ISuperAnnuationFundProvider _superAnnuationFundProvider;
        private readonly ISuperannuationFundSearchResponseConverter _annuationFundSearchResponseConverter;

        public SuperannuationFundSearch(ISuperAnnuationFundProvider superAnnuationFundProvider,
            ISuperannuationFundSearchResponseConverter annuationFundSearchResponseConverter)
        {
            _superAnnuationFundProvider = superAnnuationFundProvider;
            _annuationFundSearchResponseConverter = annuationFundSearchResponseConverter;
        }

        public IEnumerable<SuperannuationFundSearchResponse> GetSuperFundsByName(string name)
        {
            var results = _superAnnuationFundProvider.GetSuperFundsByName(name);
            return results.Select(_annuationFundSearchResponseConverter.From);
        }

        public IEnumerable<SuperannuationFundSearchResponse> GetSuperFundsByNameAndProduct(string name, string product)
        {
            var results = _superAnnuationFundProvider.GetSuperFundsByNameAndProduct(name, product);
            return results.Select(_annuationFundSearchResponseConverter.From);
        }

        public IEnumerable<SuperannuationFundSearchResponse> GetSuperFundsByABN(string abn)
        {
            var results = _superAnnuationFundProvider.GetSuperFundsByABN(abn);
            return results.Select(_annuationFundSearchResponseConverter.From);
        }

        public IEnumerable<SuperannuationFundSearchResponse> GetSuperFundsByUSI(string usi)
        {
            var results = _superAnnuationFundProvider.GetSuperFundsByUSI(usi);
            return results.Select(_annuationFundSearchResponseConverter.From);
        }

        public IEnumerable<SuperannuationFundSearchResponse> GetSuperFundsByABNAndProduct(string abn, string product)
        {
            var results = _superAnnuationFundProvider.GetSuperFundsByABNAndProduct(abn, product);
            return results.Select(_annuationFundSearchResponseConverter.From);
        }
    }
}
