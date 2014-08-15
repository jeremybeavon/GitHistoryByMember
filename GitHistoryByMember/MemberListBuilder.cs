using System;
using System.Collections.Generic;
using CSharpDom.WithSyntax;
using GitHistoryByMember.Data;

namespace GitHistoryByMember
{
    internal sealed class MemberListBuilder : SyntaxVisitor
    {
        private readonly FileCommit fileCommit;
        private readonly Action<AbstractSyntaxNode, FileCommit> buildAction;

        public MemberListBuilder(FileCommit fileCommit, Action<AbstractSyntaxNode, FileCommit> buildAction)
        {
            this.fileCommit = fileCommit;
            this.buildAction = buildAction;
        }

        public override void DefaultVisit(AbstractSyntaxNode node)
        {
            buildAction(node, fileCommit);
            base.DefaultVisit(node);
        }
    }
}
