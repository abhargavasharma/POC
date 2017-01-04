using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAL.QuoteAndApply.Rules
{
    public struct RuleRequest
    {
        public bool IsSatisfied;
        public string Message;

        public RuleRequest(bool isSatisfied, string message) : this()
        {
            IsSatisfied = isSatisfied;
            Message = message;
        }
    }
}
