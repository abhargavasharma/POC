using TAL.QuoteAndApply.Infrastructure.Crypto;

namespace TAL.QuoteAndApply.Policy.Service
{
    public interface IQuoteReferenceGenerationService
    {
        string RandomQuoteReference();
    }


    public class QuoteReferenceGenerationService : IQuoteReferenceGenerationService
    {
        public string RandomQuoteReference()
        {
            var generator = new CryptoRandom();

            var rand = generator.Next(100000000, 999999999);
            string uniqueRefNumberFormatted = rand.ToString("000000000");

            return string.Format("M{0}", uniqueRefNumberFormatted);
        }
    }
}
