using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using GitHistoryByMember.Data;

namespace GitHistoryByMember
{
    public sealed class GitRepository : IRepository
    {
        private const string LogCommand =
            "log --pretty=format:\"%H%nAuthor: %an%nDate: %ad%nSubject: %s%nCHANGES:\" --name-status --reverse --no-merges";

        private readonly string directory;

        public GitRepository(string directory)
        {
            this.directory = directory;
        }

        public IReadOnlyList<CommitFileChanges> Log()
        {
            return ExecuteCommand(LogCommand, GitLogBuilder.BuildLog);
        }

        public string FindFileAtRevision(string file, string commitId)
        {
            return ExecuteCommand(string.Format("show {0}:{1}", commitId, file), reader => reader.ReadToEnd());
        }

        private T ExecuteCommand<T>(string command, Func<TextReader, T> resultFunc)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("git", command)
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = false,
                WorkingDirectory = directory
            };
            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
                string errorText = process.StandardError.ReadToEnd();
                if (!string.IsNullOrWhiteSpace(errorText))
                {
                    throw new GitCommandFailedException(errorText);
                }

                string text = process.StandardOutput.ReadToEnd();
                using (TextReader reader = new StringReader(text))
                {
                    return resultFunc(reader);
                }
            }
        }

        private IReadOnlyList<CommitFileChanges> BuildLog(TextReader reader)
        {
            List<CommitFileChanges> log = new List<CommitFileChanges>();
            for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
            {
                CommitFileChanges commitFileChanges = new CommitFileChanges();
                commitFileChanges.Commit.CommitId = line;
                
            }

            return log;
        }
    }
}
