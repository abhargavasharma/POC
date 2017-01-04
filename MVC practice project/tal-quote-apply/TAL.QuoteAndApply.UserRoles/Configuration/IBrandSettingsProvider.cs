using System.Collections.Generic;

namespace TAL.QuoteAndApply.UserRoles.Configuration
{
    public interface IBrandSettingsProvider
    {
        IBrandConfiguration GetForBrand(string brandKey);
        IEnumerable<IBrandConfiguration> GetAllBrandConfigurations();
    }
}