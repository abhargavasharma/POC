using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAL.QuoteAndApply.Configuration.Data
{
    public interface IApplicationConfigurationSettings
    {
        string DatabaseSettingsConnectionString { get; }
    }

    public class ApplicationConfigurationSettings : IApplicationConfigurationSettings
    {
        public string DatabaseSettingsConnectionString
            => ConfigurationManager.AppSettings["Configuration.ConnectionString"];
    }
}
