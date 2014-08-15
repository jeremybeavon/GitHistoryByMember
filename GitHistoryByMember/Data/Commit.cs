using System;

namespace GitHistoryByMember.Data
{
    public sealed class Commit
    {
        public string CommitId { get; set; }

        public string Author { get; set; }

        public DateTime Date { get; set; }

        public string Subject { get; set; }

    }
}
