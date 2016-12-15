using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AGLDeveloperProgrammingChallenge.Startup))]
namespace AGLDeveloperProgrammingChallenge
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
