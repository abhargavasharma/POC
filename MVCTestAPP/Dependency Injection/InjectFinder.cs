using MVCTestAPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCTestAPP.Dependency_Injection
{
    interface InjectFinder
    {
        void injectFinder(User finder);
    }

    public interface InjectFinderUsername
    {
        void injectUsername(String userName);
    }
}
