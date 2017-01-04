

using System.Configuration;

namespace TAL.QuoteAndApply.Party.Configuration
{
    public interface IPartyConfigurationProvider
    {
        string ConnectionString { get; }
    }

    public class PartyConfigurationProvider : IPartyConfigurationProvider
    {
        public string ConnectionString
        {
            get { return ConfigurationManager.AppSettings["Party.SqlConnectionString"]; }
        }
    }
}
