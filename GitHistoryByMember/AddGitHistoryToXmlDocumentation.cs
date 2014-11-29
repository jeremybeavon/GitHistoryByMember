using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace GitHistoryByMember
{
    public sealed class AddGitHistoryToXmlDocumentation : Task
    {
        [Required]
        public string SolutionFile { get; set; }

        [Required]
        public string DocumentationFolder { get; set; }

        public override bool Execute()
        {
            return true;
        }
    }
}
