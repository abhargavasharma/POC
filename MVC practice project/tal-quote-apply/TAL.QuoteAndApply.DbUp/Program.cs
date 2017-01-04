using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using DbUp;
using DbUp.Engine;

namespace TAL.QuoteAndApply.DbUp
{
    class Program
    {
        private const string PRODUCTION_FOLDER = "Production";
        private const string QA_FOLDER = "QA";
        private const string QA_PARAMETER = "QA";

        private const int RETURN_CRAP_ARGS = -1;
        private const int RETURN_PRODUCTION_SCRIPTS_FAILED = -2;
        private const int RETURN_QA_SCRIPTS__FAILED = -3;
        private const int RETURN_SUCCESS = 0;

        static int Main(string[] args)
        {
            var options = new DeploymentOptions();
            if (!PopulateOptions(args, options))
            {
                return RETURN_CRAP_ARGS;
            }

            TimeSpan? timeout = null;
            if (options.Timeout >= 0)
                timeout = TimeSpan.FromSeconds(options.Timeout);

            var result = PerformUpgradeForEnvironment(options.ConnectionString, timeout, PRODUCTION_FOLDER);

            if (!result.Successful)
            {
                PrintError(result);
                return RETURN_PRODUCTION_SCRIPTS_FAILED;
            }


            if (options.Environment == DeploymentEnvironment.QA)
            {
                result = PerformUpgradeForEnvironment(options.ConnectionString, timeout, QA_FOLDER);

                if (!result.Successful)
                {
                    PrintError(result);
                    return RETURN_QA_SCRIPTS__FAILED;
                }

            }

            PrintSuccess();
            return RETURN_SUCCESS;

        }

        private static bool PopulateOptions(string[] args, DeploymentOptions options)
        {
            if (!CommandLine.Parser.Default.ParseArguments(args, options))
                return false;

            if (!options.Environment.HasValue)
            {
                var appSettingsEnvironment = ConfigurationManager.AppSettings["Environment"];
                options.Environment = (DeploymentEnvironment)Enum.Parse(typeof(DeploymentEnvironment), appSettingsEnvironment);
            }

            if (String.IsNullOrEmpty(options.ConnectionString))
            {
                options.ConnectionString = ConfigurationManager.ConnectionStrings["DatabaseConnection"].ConnectionString;
            }

            return true;
        }

        private static void PrintSuccess()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
        }

        private static void PrintError(DatabaseUpgradeResult result)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(result.Error);
            Console.ResetColor();
        }

        private static DatabaseUpgradeResult PerformUpgradeForEnvironment(string connectionString, TimeSpan? timeout, string folderFilter)
        {
            var upgrader =
                DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithExecutionTimeout(timeout)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), s => s.Contains("." + folderFilter + "."))
                    .LogToConsole()
                    .WithVariable("ExecutionPath", AssemblyDirectory)
                    .Build();

            return upgrader.PerformUpgrade();
        }

        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
