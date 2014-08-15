using System;

namespace GitHistoryByMember.Data
{
    public sealed class FileCommit : IEquatable<FileCommit>
    {
        public string Name { get; set; }

        public FileChangeType ChangeType { get; set; }

        public Commit Commit { get; set; }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FileCommit);
        }

        public bool Equals(FileCommit other)
        {
            return other != null && ToString() == other.ToString();
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Commit.CommitId);
        }
    }
}
