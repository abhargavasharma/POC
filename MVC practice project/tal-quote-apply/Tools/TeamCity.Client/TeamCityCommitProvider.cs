using System;
using System.Collections.Generic;
using System.Linq;
using TeamCity.Client.Model;

namespace TeamCity.Client
{
    public interface ITeamCityCommitProvider
    {
        IEnumerable<Commit> GetCommits(string buildNumber, string buildTypeId);
    }

    public class TeamCityCommitProvider : ITeamCityCommitProvider
    {
        private readonly ITeamCityBuildCrawler _service;

        public TeamCityCommitProvider()
            : this(new TeamCityBuildCrawler("http://teamcity")) // need to make this configurable
        {
            
        }

        public TeamCityCommitProvider(ITeamCityBuildCrawler service)
        {
            _service = service;
        }

        public IEnumerable<Commit> GetCommits(string buildNumber, string buildTypeId)
        {
            var build = _service.GetBuilds(buildTypeId)
                    .FirstOrDefault(b => b.number.EndsWith(buildNumber, StringComparison.OrdinalIgnoreCase));

            if (build != null)
            {
                return GetCommitsForBuild(build);
            }
            return new List<Commit>();
        }

        private IEnumerable<Commit> GetCommitsForBuild(buildsBuild build)
        {
            var changes = _service.GetChangesForBuild(build.id).Select(ch => _service.GetChange(ch.id));
            return changes.Select(change => new Commit
            {
                BuildNumber = build.number,
                CommitId = change.version,
                CommitMessage = change.comment,
                FilesChanged = change.files.Select(f => f.file).ToList()
            });
        }
    }
}
