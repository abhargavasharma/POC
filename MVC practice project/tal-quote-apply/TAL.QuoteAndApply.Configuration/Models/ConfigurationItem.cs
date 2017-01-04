using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAL.QuoteAndApply.DataModel;

namespace TAL.QuoteAndApply.Configuration.Models
{
    public class ConfigurationItem : DbItem
    {
        public int BrandId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }
}
