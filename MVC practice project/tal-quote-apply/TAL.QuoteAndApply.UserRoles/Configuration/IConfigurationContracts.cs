using System.Collections.Generic;

namespace TAL.QuoteAndApply.UserRoles.Configuration
{
    public interface IBrandConfiguration 
    {
        string BrandKey { get; }
        bool Enabled { get; }
        IBrandRoleSettings RoleSettings { get; }
    }

    public interface IBrandRoleSettings
    {
        string UnderwritingGroup { get; }
        string AgentGroup { get; }
        string ReadOnlyGroup { get; }
    }

    public interface IBrandSettingsProvider
    {
        IBrandConfiguration GetForBrand(string brandKey);
        IEnumerable<IBrandConfiguration> GetAllBrands();
        string GetUnderwriterGroup();
    }
}