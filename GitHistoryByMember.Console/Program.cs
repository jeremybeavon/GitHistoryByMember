using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GitHistoryByMember.Console
{
    public static class Program
    {
        private const string Directory = @"C:\Users\beavon\Documents\Visual Studio 2013\Projects\WcfThreadlessChannel";

        public static void Main(string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
            new HistoryBuilder(new GitRepository(Directory)).BuildXmlDocumentationAsync(
                Path.Combine(Directory, "WcfThreadlessChannel.sln"),
                Path.Combine(Directory, @"WcfThreadlessChannel\bin\Release")).Wait();
        }
    }
}
