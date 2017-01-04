
namespace TAL.QuoteAndApply.ServiceLayer.Payment.Models.Conveters
{
    public interface ICreditCardTypeConverter
    {
        CreditCardType From(DataModel.Payment.CreditCardType creditCardType);
        DataModel.Payment.CreditCardType From(CreditCardType creditCardType);
    }

    public class CreditCardTypeConverter : ICreditCardTypeConverter
    {
        public DataModel.Payment.CreditCardType From(CreditCardType creditCardType)
        {
            return (DataModel.Payment.CreditCardType)creditCardType;
        }

        public CreditCardType From(DataModel.Payment.CreditCardType creditCardType)
        {
            return (CreditCardType)creditCardType;
        }
    }
}
