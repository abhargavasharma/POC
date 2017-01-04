using System.Collections.Generic;

namespace TAL.QuoteAndApply.UserRoles.Services
{
    public class UserDetails
    {
        public string Name { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string EmailAddress { get; set; }
        public List<string> Groups { get; set; } 
    }
}