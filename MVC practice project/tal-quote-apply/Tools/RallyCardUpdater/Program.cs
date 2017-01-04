using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using Rally.ServiceLayer.Rally;
using RallyCardUpdater.Models;
using TeamCity.Client;
using TeamCity.Client.Model;

namespace RallyCardUpdater
{
    class Program
    {

        static void Main(string[] args)
        {
            var options = new CommandLineArgs();
            var result = Parser.Default.ParseArguments(args, options);
            if (result)
            {
                Do(options.BuildNumber, options.BuildType);
            }
        }

        static void Do(string buildNumber, string buildTypeId)
        {
            Console.WriteLine("Checking commits for build {0} on project {1}", buildNumber, buildTypeId);
            var commitProvider = new TeamCityCommitProvider();
            var usUpdate = new RallyUserStoryNotesUpdate();
            var deUpdate = new RallyDefectNotesUpdate();
            var commits = commitProvider.GetCommits(buildNumber, buildTypeId).ToArray();
            Console.WriteLine("{0} commits in build", commits.Length);


            var rallyCommits = commits.Select(GetRallyCommits);
            foreach (var commit in rallyCommits)
            {
                
                foreach (var cardId in commit.CardIds.Where(c => c.CardType == RallyCardType.Defect))
                {
                    Console.WriteLine("Updating Defect {0}", cardId.Id);
                    var defectMessage = GetDefectBuildMessage(commit);
                    deUpdate.UpdateDefect(cardId.Id, defectMessage);
                }
                foreach (var cardId in commit.CardIds.Where(c => c.CardType == RallyCardType.UserStory))
                {
                    Console.WriteLine("Updating User Story {0}", cardId.Id);
                    var defectMessage = GetStoryBuildMessage(commit);
                    usUpdate.UpdateStory(cardId.Id, defectMessage);
                }
            }
        }

        public static RallyCommit GetRallyCommits(Commit commit)
        {
            var newCommit = new RallyCommit(commit);

            var regexDefect = new Regex(@"(DE\d+)", RegexOptions.IgnoreCase);
            var matches = regexDefect.Matches(commit.CommitMessage);
            foreach (Match match in matches.Cast<Match>().Where(match => match.Groups.Count >= 1))
            {
                var id = match.Groups[0].Value.Replace("DE", "");
                newCommit.CardIds.Add(new CardId(id, RallyCardType.Defect));
            }
            var regexUserStory = new Regex(@"(US\d+)", RegexOptions.IgnoreCase);
            matches = regexUserStory.Matches(commit.CommitMessage);
            foreach (Match match in matches.Cast<Match>().Where(match => match.Groups.Count >= 1))
            {
                var id = match.Groups[0].Value.Replace("US", "");
                newCommit.CardIds.Add(new CardId(id, RallyCardType.UserStory));
            }

            return newCommit;
        }

        public static string GetDefectBuildMessage(RallyCommit commit)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<br />");
            sb.AppendFormat("<div><strong>-- Defect Committed -- {0} --</strong></div>", DateTime.Now).AppendLine();
            sb.AppendFormat("<div>Build Number: {0} </div>", commit.BuildNumber).AppendLine();
            sb.AppendFormat("<div>Commit Message:  {0} </div>", commit.CommitMessage).AppendLine();
            sb.AppendLine("<br />");

            return sb.ToString();
        }

        public static string GetStoryBuildMessage(RallyCommit commit)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<br />");
            sb.AppendFormat("<div><strong>-- Story Committed -- {0} --</strong></div>", DateTime.Now).AppendLine();
            sb.AppendFormat("<div>Build Number: {0} </div>", commit.BuildNumber).AppendLine();
            sb.AppendFormat("<div>Commit Message:  {0} </div>", commit.CommitMessage).AppendLine();
            sb.AppendLine("<br />");

            return sb.ToString();
        }
    }

    public class CommandLineArgs
    {
        [Option('n', "buildnumber", Required = true, HelpText = "Team City Build Number")]
        public string BuildNumber { get; set; }

        [Option('t', "buildtype", Required = true, HelpText = "Team City build Type Id")]
        public string BuildType { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
