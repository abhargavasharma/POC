using CommandLine;
using CommandLine.Text;

namespace TAL.QuoteAndApply.DbUp
{
    public class DeploymentOptions
    {
        [Option('c', "connectionstring", Required = false, DefaultValue = "",
            HelpText = "The connection string for the DB we are deploying to.")]
        public string ConnectionString { get; set; }
        [Option('e', "environment", Required = false, DefaultValue = null,
            HelpText = "The environment we are deploying to.")]
        public DeploymentEnvironment? Environment { get; set; }

        [Option('t', "timeout", Required = false, DefaultValue = -1, HelpText = "Database connection timeout. 0 no timeout, -1 default 30s, >0 Xs")]
        public int Timeout { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this).ToString();

        }
    }
}