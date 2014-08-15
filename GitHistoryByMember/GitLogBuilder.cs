using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using GitHistoryByMember.Data;

namespace GitHistoryByMember
{
    internal static class GitLogBuilder
    {
        private const string FileChangePattern = @"^(?<ChangeType>[AMD])\s+(?<Name>.+)$";
        private const RegexOptions FileChangeOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase;
        private static readonly Regex fileChangeRegex = new Regex(FileChangePattern, FileChangeOptions);

        public static IReadOnlyList<CommitFileChanges> BuildLog(TextReader reader)
        {
            List<CommitFileChanges> log = new List<CommitFileChanges>();
            CommitFileChanges commitFileChanges = null;
            using (IEnumerator<string> lines = GetLines(reader, () => commitFileChanges).GetEnumerator())
            {
                bool isEndOfText = false;
                while (!isEndOfText)
                {
                    commitFileChanges = new CommitFileChanges();
                    if (!ReadCommitId(lines, commitFileChanges))
                    {
                        break;
                    }

                    ReadAuthor(lines, commitFileChanges);
                    ReadDate(lines, commitFileChanges);
                    ReadSubject(lines, commitFileChanges);
                    isEndOfText = ReadFileChanges(lines, commitFileChanges);
                    log.Add(commitFileChanges);
                }
            }

            return log;
        }

        private static IEnumerable<string> GetLines(TextReader reader, Func<CommitFileChanges> commitFileChangesFunc)
        {
            string line = reader.ReadLine();
            while (line != null)
            {
                commitFileChangesFunc().AddToLog(line);
                yield return line;
                line = reader.ReadLine();
            }
        }

        private static bool ReadCommitId(IEnumerator<string> lines, CommitFileChanges commitFileChanges)
        {
            bool result = lines.MoveNext();
            if (result)
            {
                commitFileChanges.Commit.CommitId = lines.Current;
            }

            return result;
        }

        private static void ReadAuthor(IEnumerator<string> lines, CommitFileChanges commitFileChanges)
        {
            bool hasLine = lines.MoveNext();
            if (!hasLine)
            {
                throw new GitCommandFailedException("Expected Author but end of file was found", commitFileChanges);
            }

            if (!lines.Current.StartsWith("Author: "))
            {
                string message = string.Format("Expected Author but found: {0}", lines.Current);
                throw new GitCommandFailedException(message, commitFileChanges);
            }

            commitFileChanges.Commit.Author = lines.Current.Substring("Author: ".Length);
        }

        private static void ReadDate(IEnumerator<string> lines, CommitFileChanges commitFileChanges)
        {
            bool hasLine = lines.MoveNext();
            if (!hasLine)
            {
                throw new GitCommandFailedException("Expected Date but end of file was found", commitFileChanges);
            }

            if (!lines.Current.StartsWith("Date: "))
            {
                string message = string.Format("Expected Date but found: {0}", lines.Current);
                throw new GitCommandFailedException(message, commitFileChanges);
            }

            DateTime date;
            string dateText = lines.Current.Substring("Date: ".Length);
            if (!DateTime.TryParseExact(dateText, "ddd MMM dd HH:mm:ss yyyy K", null, DateTimeStyles.None, out date))
            {
                string message = string.Format("Could not parse date: {0}", lines.Current);
                throw new GitCommandFailedException(message, commitFileChanges);
            }

            commitFileChanges.Commit.Date = date;
        }

        private static void ReadSubject(IEnumerator<string> lines, CommitFileChanges commitFileChanges)
        {
            bool hasLine = lines.MoveNext();
            if (!hasLine)
            {
                throw new GitCommandFailedException("Expected Subject but end of file was found", commitFileChanges);
            }

            if (!lines.Current.StartsWith("Subject: "))
            {
                string message = string.Format("Expected Subject but found: {0}", lines.Current);
                throw new GitCommandFailedException(message, commitFileChanges);
            }

            StringBuilder subject = new StringBuilder(lines.Current.Substring("Subject: ".Length));
            bool areChangesFound = false;
            while (lines.MoveNext())
            {
                if (lines.Current == "CHANGES:")
                {
                    areChangesFound = true;
                    break;
                }

                subject.AppendLine(lines.Current);
            }

            if (!areChangesFound)
            {
                throw new GitCommandFailedException("Expected CHANGES but end of file was found", commitFileChanges);
            }

            commitFileChanges.Commit.Subject = subject.ToString();
        }

        private static bool ReadFileChanges(IEnumerator<string> lines, CommitFileChanges commitFileChanges)
        {
            while (lines.MoveNext())
            {
                if (lines.Current.Length == 0)
                {
                    return false;
                }

                Match fileChangeMatch = fileChangeRegex.Match(lines.Current);
                if (!fileChangeMatch.Success)
                {
                    string message = string.Format("Expected file change but found: {0}", lines.Current);
                    throw new GitCommandFailedException(message, commitFileChanges);
                }

                FileCommit fileCommit = new FileCommit()
                {
                    ChangeType = GetFileChangeType(fileChangeMatch.Groups["ChangeType"].Value[0]),
                    Name = fileChangeMatch.Groups["Name"].Value,
                    Commit = commitFileChanges.Commit
                };
                commitFileChanges.FileCommits.Add(fileCommit);
            }

            return true;
        }

        private static FileChangeType GetFileChangeType(char changeType)
        {
            switch (char.ToUpper(changeType))
            {
                case 'A':
                    return FileChangeType.Added;
                case 'M':
                    return FileChangeType.Modified;
                default:
                    return FileChangeType.Deleted;
            }
        }
    }
}
