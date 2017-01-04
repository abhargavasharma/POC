using System.Collections.Generic;

namespace TeamCity.Client.Model
{
    public class Commit
    {
        public string BuildNumber { get; set; }
        public string CommitId { get; set; }
        public string CommitMessage { get; set; }
        public List<string> FilesChanged { get; set; } 
    }
}