using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVCTestAPP.Models;

namespace MVCTestAPP.Dependency_Injection
{
    public class Lister:InjectFinder
    {
        User finder = null;
        public void injectFinder(User finder)
        {
            this.finder = finder;
        }
    }

    class UserFinder:Lister,InjectFinderUsername
    {
        String userName = null;
        public void injectUsername(String userName)
        {
            this.userName = userName;
        }
    }
}