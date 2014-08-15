using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHistoryByMember
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <list type=""
    /// </remarks>
    public sealed class FileSystemGitHistoryDataAccess : IGitHistoryDataAccess
    {
        public void UpdateXmlForCommit(string commitId, string xml)
        {
            throw new NotImplementedException();
        }

        public void UpdateXmlForMethod(string fullTypeName, string methodSignature, string xml)
        {
            throw new NotImplementedException();
        }

        public string GetXmlForCommit(string commitId)
        {
            throw new NotImplementedException();
        }

        public string GetXmlForMethod(string fullTypeName, string methodSignature)
        {
            throw new NotImplementedException();
        }
    }
}

/*
Commits
	Commit
		Commit Id
		Author
		Date
        Subject
		Assemblies
			Assembly
				Namespaces
					Namespace
						Types
							Type
								Members
									Member

Member
    Commits
        Commit
            CommitId
            Author
            Date
            Subject

*/
