using System.Collections.Generic;
using System.Threading.Tasks;
using CSharpDom.WithSymbols;
using Microsoft.CodeAnalysis;

namespace GitHistoryByMember
{
    internal sealed class DocumentationIdBuilder : SymbolsVisitor
    {
        private readonly IDictionary<string, string> documentationIds;
        private readonly IDictionary<string, string> documentationIdsByProject;
        private string currentProject;

        public DocumentationIdBuilder(
            IDictionary<string, string> documentationIds,
            IDictionary<string, string> documentationIdsByProject)
        {
            this.documentationIds = documentationIds;
            this.documentationIdsByProject = documentationIdsByProject;
        }

        public override Task VisitProjectAsync(ProjectWithSymbols node)
        {
            currentProject = node.Project.AssemblyName;
            return base.VisitProjectAsync(node);
        }

        public override void VisitClass(ClassWithSymbols node)
        {
            AddDocumentationId(node, node.Symbol);
            base.VisitClass(node);
        }

        public override void VisitConstructor(ConstructorWithSymbols node)
        {
            AddDocumentationId(node, node.Symbol);
            base.VisitConstructor(node);
        }

        public override void VisitConversionOperator(ConversionOperatorWithSymbols node)
        {
            AddDocumentationId(node, node.Symbol);
            base.VisitConversionOperator(node);
        }

        public override void VisitDelegate(DelegateWithSymbols node)
        {
            AddDocumentationId(node, node.Symbol);
            base.VisitDelegate(node);
        }

        public override void VisitDestructor(DestructorWithSymbols node)
        {
            AddDocumentationId(node, node.Symbol);
            base.VisitDestructor(node);
        }

        public override void VisitEnum(EnumWithSymbols node)
        {
            AddDocumentationId(node, node.Symbol);
            base.VisitEnum(node);
        }

        public override void VisitEnumMember(EnumMemberWithSymbols node)
        {
            AddDocumentationId(node, node.Symbol);
            base.VisitEnumMember(node);
        }

        public override void VisitEvent(EventWithSymbols node)
        {
            AddDocumentationId(node, node.Symbol);
            base.VisitEvent(node);
        }

        public override void VisitEventProperty(EventPropertyWithSymbols node)
        {
            AddDocumentationId(node, node.Symbol);
            base.VisitEventProperty(node);
        }

        public override void VisitField(FieldWithSymbols node)
        {
            AddDocumentationId(node, node.Symbol);
            base.VisitField(node);
        }

        public override void VisitIndexer(IndexerWithSymbols node)
        {
            AddDocumentationId(node, node.Symbol);
            base.VisitIndexer(node);
        }

        public override void VisitInterface(InterfaceWithSymbols node)
        {
            AddDocumentationId(node, node.Symbol);
            base.VisitInterface(node);
        }

        public override void VisitMethod(MethodWithSymbols node)
        {
            AddDocumentationId(node, node.Symbol);
            base.VisitMethod(node);
        }

        public override void VisitNestedClass(NestedClassWithSymbols node)
        {
            AddDocumentationId(node, node.Symbol);
            base.VisitNestedClass(node);
        }

        public override void VisitNestedDelegate(NestedDelegateWithSymbols node)
        {
            AddDocumentationId(node, node.Symbol);
            base.VisitNestedDelegate(node);
        }

        public override void VisitNestedDestructor(NestedDestructorWithSymbols node)
        {
            AddDocumentationId(node, node.Symbol);
            base.VisitNestedDestructor(node);
        }

        public override void VisitNestedEnum(NestedEnumWithSymbols node)
        {
            AddDocumentationId(node, node.Symbol);
            base.VisitNestedEnum(node);
        }

        public override void VisitNestedEnumMember(NestedEnumMemberWithSymbols node)
        {
            AddDocumentationId(node, node.Symbol);
            base.VisitNestedEnumMember(node);
        }

        public override void VisitNestedInterface(NestedInterfaceWithSymbols node)
        {
            AddDocumentationId(node, node.Symbol);
            base.VisitNestedInterface(node);
        }

        public override void VisitNestedStruct(NestedStructWithSymbols node)
        {
            AddDocumentationId(node, node.Symbol);
            base.VisitNestedStruct(node);
        }

        public override void VisitOperatorOverload(OperatorOverloadWithSymbols node)
        {
            AddDocumentationId(node, node.Symbol);
            base.VisitOperatorOverload(node);
        }

        public override void VisitProperty(PropertyWithSymbols node)
        {
            AddDocumentationId(node, node.Symbol);
            base.VisitProperty(node);
        }

        public override void VisitStruct(StructWithSymbols node)
        {
            AddDocumentationId(node, node.Symbol);
            base.VisitStruct(node);
        }

        private void AddDocumentationId(AbstractSymbolNode node, ISymbol symbol)
        {
            string documentationId = symbol.GetDocumentationCommentId();
            documentationIds.Add(node.ToString(), documentationId);
            documentationIdsByProject.Add(documentationId, currentProject);
        }
    }
}
