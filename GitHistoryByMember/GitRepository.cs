using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using GitHistoryByMember.Data;

namespace GitHistoryByMember
{
    public sealed class GitRepository : IRepository
    {
        private const string LogCommandFormat = 
            "@ECHO OFF\r\n" +
            "git log " +
            "--pretty=format:\"%%H%%nAuthor: %%an%%nDate: %%ad%%nSubject: %%s%%nCHANGES:\" " +
            "--name-status " +
            "--reverse " +
            "--no-merges " +
            "> \"{0}\" 2>&1";

        private const string ShowCommandFormat = "@ECHO OFF\r\ngit show %1 > \"{0}\" 2>&1";

        private readonly string directory;
        private readonly string logCommandFile;
        private readonly string logOutput;
        private readonly string showCommandFile;
        private readonly string showOutput;

        public GitRepository(string directory)
        {
            this.directory = directory;
            string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            logCommandFile = Path.Combine(assemblyDirectory, "gitlog.bat");
            logOutput = Path.Combine(assemblyDirectory, "gitlog.txt");
            File.WriteAllText(logCommandFile, string.Format(LogCommandFormat, logOutput));
            showCommandFile = Path.Combine(assemblyDirectory, "gitshow.bat");
            showOutput = Path.Combine(assemblyDirectory, "gitshow.txt");
            File.WriteAllText(showCommandFile, string.Format(ShowCommandFormat, showOutput));
        }

        public IReadOnlyList<CommitFileChanges> Log()
        {
            ProcessStartInfo processStart = new ProcessStartInfo(logCommandFile)
            {
                WorkingDirectory = directory,
                CreateNoWindow = false,
                UseShellExecute = false
            };
            using (Process process = Process.Start(processStart))
            {
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    throw new GitCommandFailedException();
                }
            }

            using (TextReader reader = new StringReader(File.ReadAllText(logOutput)))
            {
                return GitLogBuilder.BuildLog(reader);
            }
        }

        public string FindFileAtRevision(string file, string commitId)
        {
            ProcessStartInfo processStart = new ProcessStartInfo(showCommandFile)
            {
                WorkingDirectory = directory,
                CreateNoWindow = false,
                UseShellExecute = false,
                Arguments = string.Format("{0}:{1}", commitId, file)
            };
            using (Process process = Process.Start(processStart))
            {
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    throw new GitCommandFailedException();
                }
            }

            return File.ReadAllText(showOutput);
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
