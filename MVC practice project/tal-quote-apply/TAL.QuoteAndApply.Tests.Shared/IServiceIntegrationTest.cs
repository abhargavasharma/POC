using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAL.QuoteAndApply.Tests.Shared
{
    public interface IServiceIntegrationTest<T>
    {
        void Setup();
        T GetServiceInstance();
    }
}
