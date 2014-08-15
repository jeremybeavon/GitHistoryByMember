using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GitHistoryByMember.Console
{
    public static class Program
    {
        private const string Directory = @"C:\Users\beavon\Documents\Visual Studio 2013\Projects\MSTestCaseExtensions";

        public static void Main(string[] args)
        {
            new HistoryBuilder(new GitRepository(Directory)).BuildXmlDocumentationAsync(
                Path.Combine(Directory, "MSTestCaseExtensions.sln"),
                Path.Combine(Directory, @"MSTestCaseExtensions\bin\Release")).Wait();
        }
    }
}
