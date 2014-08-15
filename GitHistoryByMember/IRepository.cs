using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitHistoryByMember.Data;

namespace GitHistoryByMember
{
    public interface IRepository
    {
        IReadOnlyList<CommitFileChanges> Log();

        string FindFileAtRevision(string file, string commitId);
    }
}
