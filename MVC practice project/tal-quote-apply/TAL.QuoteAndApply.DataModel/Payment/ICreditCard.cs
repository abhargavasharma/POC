using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAL.QuoteAndApply.DataModel.Payment
{
    public interface ICreditCard
    {
        CreditCardType CardType { get; set; }
        string NameOnCard { get; set; }
        string CardNumber { get; set; }
        string ExpiryMonth { get; set; }
        string ExpiryYear { get; set; }
        string Token { get; set; }
    }
}
