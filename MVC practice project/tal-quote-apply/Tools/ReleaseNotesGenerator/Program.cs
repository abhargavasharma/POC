using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using CommandLine;
using Rally.ServiceLayer.Rally;
using TeamCity.Client;
using TeamCity.Client.Model;

namespace ReleaseNotesGenerator
{
    public class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            var result = CommandLine.Parser.Default.ParseArguments(args, options);

            if (!result || options.Help)
            {
                Console.WriteLine(CommandLine.Text.HelpText.AutoBuild(options));
                return;
            }

            new ReleaseNotesGeneratorRunner().Run(options);
        }
    }

    public class ReleaseNotesGeneratorRunner
    {
        private const string RELEASE_NOTES_EXTENSION = ".md";
        private const string RELEASE_TEMPLATE_NAME = "talconsumereleasetemplate.md";
        private readonly string BASEDIR = Path.GetFullPath(".");
        private const string HOME_PAGE_FILE = "home.md";

        public void Run(Options options)
        {
            var workingDir = Path.Combine(BASEDIR, "working");

            DeleteReadOnlyDirectory(workingDir);
            Environment.SetEnvironmentVariable("HOME", Path.GetFullPath("profile"));

            var result = RunGitCommand(BASEDIR, "clone git@tdgitlab:tal/tal-quote-apply.wiki.git working");

            var releaseNotesFileName = $"TALConsumerRelease{options.ParsedReleaseDate.ToString("yyyyMMdd")}";

            if (ReleaseNotesAlreadyExist(releaseNotesFileName, workingDir) && !options.DryRun)
            {
                ConsoleLogInfo("Release notes for this build have already been created");
                Environment.Exit(0);
            }

            bool commitsContainDbChanges = false;
            Commit[] commits = new Commit[0];
            try
            {
                commits = GetChangesFromGit(options.BuildNumber, options.BuildType).ToArray();
                commitsContainDbChanges = IsDatabaseChangesInCommits(commits, options.DatabaseChangesRegex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting changes from TeamCity. Skipping step. Reason: {0}", ex.Message);
            }

            UserStory[] stories = new UserStory[0];
            Defect[] defects = new Defect[0];

            try
            {
                var usService = new RallyUserStoryProvider();
                if (commits.Length > 0)
                {
                    var userStoryIds = commits.SelectMany(commit => GetUserStoriesFromCommits(commit).Distinct()).Distinct().ToArray();
                    stories = usService.GetUserStories(userStoryIds).ToArray();
                    var defectIds = commits.SelectMany(commit => GetUserStoriesFromCommits(commit).Distinct()).Distinct().ToArray();
                    defects = usService.GetDefects(defectIds).ToArray();
                }
                else
                {
                    var itService = new RallyIterationProvider();
                    var iteration = itService.GetCurrentIteration("MMD - New Business Team");
                    
                    stories = usService.GetUserStoriesForIteration(iteration, "In Dev");
                    defects = usService.GetDefectsForIteration(iteration, "In Dev");
                }

                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting user stories from rally. Skipping step. Reason: {0}", ex.Message);
            }

            var template = GetTemplateContents(workingDir);
            var releaseNotes = SubsituteReleaseValuesIntoTemplate(template, options, stories, defects, !commitsContainDbChanges);

            WriteReleaseNotes(workingDir, releaseNotesFileName, releaseNotes);

            var releaseNotesLabel = $"TAL Consumer {options.ParsedReleaseDate.ToString("dd/MM/yyyy")}";
            AppendLinkToReleaseNotes(workingDir, releaseNotesLabel, releaseNotesFileName);

            result = RunGitCommand(workingDir, $"add --all");
            result = RunGitCommand(workingDir, $"status");

            if (result.ExitCode == 0 && result.Output.Contains("Changes to be committed"))
            {
                Console.WriteLine($"Dry run is {options.DryRun}");
                result = RunGitCommand(workingDir, $"commit -m \"{releaseNotesFileName}\"");

                if (!options.DryRun)
                {
                    result = RunGitCommand(workingDir, "push origin master");
                }
                else
                {
                    Console.WriteLine("Dry run, did not push");
                }
            }
            
            Console.WriteLine("Done");
        }

        public static IEnumerable<string> GetUserStoriesFromCommits(Commit commit)
        {
            var regexUserStory = new Regex(@"(US\d+)", RegexOptions.IgnoreCase);
            var matches = regexUserStory.Matches(commit.CommitMessage);
            foreach (Match match in matches.Cast<Match>().Where(match => match.Groups.Count >= 1))
            {
                yield return match.Groups[0].Value;
            }
        }

        public static IEnumerable<string> GetDefectsFromCommits(Commit commit)
        {
            var regexUserStory = new Regex(@"(DE\d+)", RegexOptions.IgnoreCase);
            var matches = regexUserStory.Matches(commit.CommitMessage);
            foreach (Match match in matches.Cast<Match>().Where(match => match.Groups.Count >= 1))
            {
                yield return match.Groups[0].Value;
            }
        }

        public static Commit[] GetChangesFromGit(string buildNumber, string buildTypeId)
        {
            var commitProvider = new TeamCityCommitProvider();
            return commitProvider.GetCommits(buildNumber, buildTypeId).ToArray();
        }

        public static bool IsDatabaseChangesInCommits(Commit[] commits, string commitFolderRegexMatch)
        {
            return commits.SelectMany(c => c.FilesChanged)
                .Any(fullFileNameAndPath => Regex.IsMatch(fullFileNameAndPath, commitFolderRegexMatch, RegexOptions.IgnoreCase));
        }

        /// <summary>
        /// Recursively deletes a directory as well as any subdirectories and files. If the files are read-only, they are flagged as normal and then deleted.
        /// </summary>
        /// <param name="directory">The name of the directory to remove.</param>
        public static void DeleteReadOnlyDirectory(string directory)
        {
            if (!Directory.Exists(directory))
                return;

            foreach (var subdirectory in Directory.EnumerateDirectories(directory))
            {
                DeleteReadOnlyDirectory(subdirectory);
            }
            foreach (var fileName in Directory.EnumerateFiles(directory))
            {
                var fileInfo = new FileInfo(fileName);
                fileInfo.Attributes = FileAttributes.Normal;
                fileInfo.Delete();
            }
            Directory.Delete(directory);
        }

        private bool ReleaseNotesAlreadyExist(string releaseNotesFilename, string workingDir)
        {
            var filename = $"{workingDir}\\{releaseNotesFilename}{RELEASE_NOTES_EXTENSION}";
            return File.Exists(filename);
        }

        private void ConsoleLogInfo(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        private string SubsituteReleaseValuesIntoTemplate(string templateContents, Options options, IEnumerable<UserStory> stories, IEnumerable<Defect> defects, bool isNoDownTimeDeployment)
        {
            var releaseContents = new StringBuilder();
            foreach (var userStory in stories)
            {
                releaseContents.Append("|").Append(userStory.FriendlyId).Append("|").Append(userStory.Name).Append("|").AppendLine();
            }
            foreach (var userStory in defects)
            {
                releaseContents.Append("|").Append(userStory.FriendlyId).Append("|").Append(userStory.Name).Append("|").AppendLine();
            }

            var deploymentType = isNoDownTimeDeployment ? "No Downtime" : "**DB Changes, requires downtime**";

            var replacedContents = templateContents
                .Replace("[RELEASE_DATE]", options.ReleaseDate)
                .Replace("[DRY_RUN_DATE]", options.DryRunDate)
                .Replace("[TAL_CONSUMER_RELEASE_NO]", FormatBuildNumber(options.BuildNumber))
                .Replace("[TAL_CONSUMER_ROLLBACK_NO]", FormatBuildNumber(options.RollbackBuildNumber))
                .Replace("[DEPLOYMENT_TYPE]", deploymentType)
                .Replace("[RELEASE_CONTENTS]", releaseContents.ToString());

            return replacedContents;
        }

        private static string FormatBuildNumber(string buildNumber)
        {
            if (buildNumber.Equals("[TBC]"))
            {
                return buildNumber;
            }

            return buildNumber.StartsWith("#") ? buildNumber : $"#{buildNumber}";
        }

        private string GetTemplateContents(string workingDir)
        {
            return System.IO.File.ReadAllText($"{workingDir}/{RELEASE_TEMPLATE_NAME}");
        }

        private void AppendLinkToReleaseNotes(string workingDir, string releaseNotesLabel, string releaseNotesFileName)
        {
            var path = $"{workingDir}/{HOME_PAGE_FILE}";
            var releaseEntry = $"[{releaseNotesLabel}]({releaseNotesFileName})";

            var homeContents = File.ReadAllText(path);
            homeContents = homeContents.Replace(releaseEntry, "").Trim('\t', '\r', '\n', ' ');
            homeContents = homeContents + Environment.NewLine + Environment.NewLine + releaseEntry;

            File.WriteAllText(path, homeContents);
        }

        private void WriteReleaseNotes(string workingDir, string fileName, string fileContents)
        {
            var filePath = $@"{workingDir}\{fileName}{RELEASE_NOTES_EXTENSION}";

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            
            File.WriteAllText(filePath, fileContents);
        }

        private GitCommandResult RunGitCommand(string workingDirectory, string command)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"git {command}");
            Console.ResetColor();

            var gitInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                FileName = @"C:\Program Files (x86)\Git\bin\git.exe",
                Arguments = $"{command}",
                WorkingDirectory = workingDirectory,
                UseShellExecute = false
            };

            var gitProcess = new Process { StartInfo = gitInfo };
            gitProcess.Start();

            var processError = gitProcess.StandardError.ReadToEnd(); 
            var processOutput = gitProcess.StandardOutput.ReadToEnd();

            gitProcess.WaitForExit();
            
            Console.WriteLine(processOutput);

            if (!string.IsNullOrEmpty(processError))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(processError);
                Console.ResetColor();
            }

            if (gitProcess.ExitCode != 0)
            {
                throw new ApplicationException($"Git process failed with exit code {gitProcess.ExitCode}");
            }

            return new GitCommandResult(gitProcess.ExitCode, processOutput, processError);
        }
    }

    public class GitCommandResult
    {
        public int ExitCode { get; }
        public string Output { get; }
        public string Error { get; }

        public GitCommandResult(int exitCode, string output, string error)
        {
            ExitCode = exitCode;
            Output = output;
            Error = error;
        }
    }

    public class Options
    {
        public const string PARAM_DATE_FORMAT = "dd-MM-yyyy";

        [Option('h', "help", DefaultValue = false, Required = false)]
        public bool Help { get; set; }

        [Option("dry-run", DefaultValue = false, HelpText = "Set to true to not commit ouput of generator (for testing purposes)")]
        public bool DryRun { get; set; }

        [Option("dry-run-date", DefaultValue = PARAM_DATE_FORMAT, HelpText = "Date of the dry run", Required = true)]
        public string DryRunDate { get; set; }

        [Option("release-date", DefaultValue = PARAM_DATE_FORMAT, HelpText = "Date of the release", Required = true)]
        public string ReleaseDate { get; set; }

        [Option("build-number", DefaultValue = "[TBC]", HelpText = "Release build number", Required = false)]
        public string BuildNumber { get; set; }

        [Option("rollback-build-number", DefaultValue = "[TBC]", HelpText = "Rollback build number", Required = false)]
        public string RollbackBuildNumber { get; set; }

        [Option("build-type", HelpText = "Release build number", Required = false)]
        public string BuildType { get; set; }

        [Option("db-dir-regex", HelpText = "Release build number", Required = false)]
        public string DatabaseChangesRegex { get; set; }

        public DateTime ParsedDryRunDate => DateTime.ParseExact(DryRunDate, PARAM_DATE_FORMAT, CultureInfo.InvariantCulture);
        public DateTime ParsedReleaseDate => DateTime.ParseExact(ReleaseDate, PARAM_DATE_FORMAT, CultureInfo.InvariantCulture);
    }

}
