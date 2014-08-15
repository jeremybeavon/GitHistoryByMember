using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHistoryByMember
{
    public interface IGitHistoryDataAccess
    {
        void UpdateXmlForCommit(string commitId, string xml);

        void UpdateXmlForMethod(string fullTypeName, string methodSignature, string xml);

        string GetXmlForCommit(string commitId);

        string GetXmlForMethod(string fullTypeName, string methodSignature);
    }
}
