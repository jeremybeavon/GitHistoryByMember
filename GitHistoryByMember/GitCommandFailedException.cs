using System;
using System.Runtime.Serialization;
using System.Text;
using GitHistoryByMember.Data;

namespace GitHistoryByMember
{
    [Serializable]
    public class GitCommandFailedException : Exception
    {
        public GitCommandFailedException()
        {
        }

        public GitCommandFailedException(string message) : base(message)
        {
        }

        public GitCommandFailedException(string message, Exception inner) : base(message, inner)
        {
        }

        public GitCommandFailedException(string message, CommitFileChanges commitFileChanges)
            : this(BuildMessage(message, commitFileChanges))
        {
        }

        protected GitCommandFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        private static string BuildMessage(string message, CommitFileChanges commitFileChanges)
        {
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.AppendLine(message);
            messageBuilder.AppendLine();
            messageBuilder.AppendLine("Output:");
            messageBuilder.AppendLine(commitFileChanges.ToString());
            return messageBuilder.ToString();
        }
    }
}
