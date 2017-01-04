using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAL.QuoteAndApply.Infrastructure.Concurrency
{
    public interface IConcurrencyToken
    {
        string ConcurrencyToken { get; set; }
    }
}
