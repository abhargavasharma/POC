using System.Configuration;

namespace TAL.QuoteAndApply.Interactions.Configuration
{
    public interface IInteractionsConfigurationProvider
    {
        string ConnectionString { get; }
    }

    public class InteractionsConfigurationProvider : IInteractionsConfigurationProvider
    {
        public string ConnectionString
        {
            get { return ConfigurationManager.AppSettings["Interactions.SqlConnectionString"]; }
        }
    }
}
