using System.Collections.Generic;
using TeamCity.Client.Model;

namespace RallyCardUpdater.Models
{
    public class RallyCommit
    {
        public string BuildNumber { get; set; }
        public string CommitId { get; set; }
        public string CommitMessage { get; set; }
        public List<CardId> CardIds { get; set; }

        public RallyCommit(Commit commit)
        {
            this.BuildNumber = commit.BuildNumber;
            this.CommitId = commit.CommitId;
            this.CommitMessage = commit.CommitMessage;
            CardIds = new List<CardId>();
        }
    }
}