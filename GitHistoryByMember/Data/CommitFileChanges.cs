using System.Collections.ObjectModel;
using System.Text;

namespace GitHistoryByMember.Data
{
    public sealed class CommitFileChanges
    {
        private readonly StringBuilder log;

        public CommitFileChanges()
        {
            Commit = new Commit();
            FileCommits = new FileCommits();
            log = new StringBuilder();
        }

        public Commit Commit { get; set; }

        public FileCommits FileCommits { get; private set; }

        public void AddToLog(string line)
        {
            log.AppendLine(line);
        }

        public override string ToString()
        {
            return log.ToString();
        }
    }
}
