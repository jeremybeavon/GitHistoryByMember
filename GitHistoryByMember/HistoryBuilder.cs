using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Threading.Tasks;
using CSharpDom.WithSymbols;
using CSharpDom.WithSyntax;
using GitHistoryByMember.Data;

namespace GitHistoryByMember
{
    public class HistoryBuilder
    {
        private const string MemberXPath = "//member[@name='{0}']";
        private readonly IRepository repository;
        private readonly IDictionary<string, string> documentationIds;
        private readonly IDictionary<string, string> documentationIdsByProject;
        private readonly FileCommitsByMember fileCommitsByMember;
        private readonly IDictionary<string, SolutionWithSyntax> sourceTextByFileName;
        private readonly IDictionary<string, XmlDocument> documentationFiles;

        public HistoryBuilder(IRepository repository)
        {
            this.repository = repository;
            documentationIds = new Dictionary<string, string>();
            documentationIdsByProject = new Dictionary<string, string>();
            fileCommitsByMember = new FileCommitsByMember();
            sourceTextByFileName = new Dictionary<string, SolutionWithSyntax>();
            documentationFiles = new Dictionary<string, XmlDocument>();
        }

        public async Task<FileCommitsByMember> BuildHistoryAsync(string solutionFile)
        {
            return await BuildHistoryAsync(await SolutionWithSymbols.OpenSolutionAsync(solutionFile));
        }

        public async Task<FileCommitsByMember> BuildHistoryAsync(SolutionWithSymbols solution)
        {
            documentationIds.Clear();
            documentationIdsByProject.Clear();
            fileCommitsByMember.Clear();
            sourceTextByFileName.Clear();
            await BuildDocumentationIdsAsync(solution);
            await BuildFileCommitsByMemberAsync();
            return fileCommitsByMember;
        }

        public async Task BuildXmlDocumentationAsync(string solutionFile, string documentationPath)
        {
            await BuildHistoryAsync(solutionFile);
            documentationFiles.Clear();
            foreach (KeyValuePair<string, FileCommits> memberCommits in fileCommitsByMember)
            {
                XmlDocument document = GetDocumentationFile(documentationPath, memberCommits.Key);
                XmlNode remarksNode = GetRemarksNode(memberCommits.Key, document);
                XmlNode paraNode = document.CreateElement("para");
                remarksNode.AppendChild(paraNode);
                paraNode.AppendChild(document.CreateTextNode("Change History:"));
                XmlElement listNode = AppendTableList(paraNode, document);
                foreach (FileCommit fileCommit in memberCommits.Value.OrderByDescending(commit => commit.Commit.Date))
                {
                    AppendTableListItem(fileCommit.Commit, listNode, document);
                }
            }

            foreach (KeyValuePair<string, XmlDocument> file in documentationFiles)
            {
                file.Value.Save(Path.Combine(documentationPath, file.Key + ".xml"));
            }
        }

        private async Task BuildDocumentationIdsAsync(SolutionWithSymbols solution)
        {
            await solution.AcceptAsync(new DocumentationIdBuilder(documentationIds, documentationIdsByProject));
        }

        private async Task BuildFileCommitsByMemberAsync()
        {
            foreach (FileCommit commit in FindCommits())
            {
                switch (commit.ChangeType)
                {
                    case FileChangeType.Added:
                        SolutionWithSyntax newText = GetNewText(commit);
                        await newText.AcceptAsync(new MemberListBuilder(commit, AddFileCommit));
                        sourceTextByFileName[commit.Name] = newText;
                        break;
                    case FileChangeType.Modified:
                        foreach (AbstractSyntaxNode member in await FindChangesAsync(commit))
                        {
                            AddFileCommit(member, commit);
                        }

                        break;
                    case FileChangeType.Deleted:
                        await GetOldText(commit).AcceptAsync(new MemberListBuilder(commit, AddFileCommit));
                        break;
                }
            }
        }

        private IEnumerable<FileCommit> FindCommits()
        {
            return repository.Log().SelectMany(log => log.FileCommits).Where(commit => commit.Name.EndsWith(".cs"));
        }

        private void AddFileCommit(AbstractSyntaxNode member, FileCommit fileCommit)
        {
            string documentationId;
            if (documentationIds.TryGetValue(member.ToString(), out documentationId))
            {
                FileCommits fileCommits;
                if (!fileCommitsByMember.TryGetValue(documentationId, out fileCommits))
                {
                    fileCommits = new FileCommits();
                    fileCommitsByMember.Add(documentationId, fileCommits);
                }

                fileCommits.Add(fileCommit);
            }
        }

        private Task<IEnumerable<AbstractSyntaxNode>> FindChangesAsync(FileCommit commit)
        {
            SolutionWithSyntax oldText = GetOldText(commit);
            SolutionWithSyntax newText = GetNewText(commit);
            sourceTextByFileName[commit.Name] = newText;
            return ChangeFinder.FindChangesForSingleDocumentSolutionAsync(oldText, newText);
        }

        private SolutionWithSyntax GetOldText(FileCommit commit)
        {
            return sourceTextByFileName[commit.Name];
        }

        private SolutionWithSyntax GetNewText(FileCommit commit)
        {
            string text = repository.FindFileAtRevision(commit.Name, commit.Commit.CommitId);
            return SolutionWithSyntax.CreateSolutionForText(text);
        }

        private XmlDocument GetDocumentationFile(string documentationPath, string memberName)
        {
            string assemblyName = documentationIdsByProject[memberName];
            XmlDocument document;
            if (!documentationFiles.TryGetValue(assemblyName, out document))
            {
                document = new XmlDocument();
                document.Load(Path.Combine(documentationPath, assemblyName + ".xml"));
                documentationFiles.Add(assemblyName, document);
            }

            return document;
        }

        private XmlNode GetRemarksNode(string memberName, XmlDocument document)
        {
            XmlNode memberNode = document.SelectSingleNode(string.Format(MemberXPath, memberName));
            if (memberNode == null)
            {
                memberNode = document.CreateElement("member");
                document.SelectSingleNode("//members").AppendChild(memberNode);
                XmlAttribute nameAttribute = document.CreateAttribute("name");
                memberNode.Attributes.Append(nameAttribute);
                nameAttribute.Value = memberName;
            }

            XmlNode remarksNode = memberNode.ChildNodes.Cast<XmlNode>().FirstOrDefault(node => node.Name == "remarks");
            if (remarksNode == null)
            {
                remarksNode = document.CreateElement("remarks");
                memberNode.AppendChild(remarksNode);
            }

            return remarksNode;
        }

        private XmlElement AppendTableList(XmlNode paraNode, XmlDocument document)
        {
            XmlElement listNode = document.CreateElement("list");
            paraNode.AppendChild(listNode);
            XmlAttribute typeAttribute = document.CreateAttribute("type");
            typeAttribute.Value = "table";
            listNode.Attributes.Append(typeAttribute);
            XmlElement listHeaderNode = document.CreateElement("listheader");
            listNode.AppendChild(listHeaderNode);
            XmlElement termNode = document.CreateElement("term");
            listHeaderNode.AppendChild(termNode);
            termNode.InnerText = "Author & Date";
            XmlElement descriptionNode = document.CreateElement("description");
            listHeaderNode.AppendChild(descriptionNode);
            descriptionNode.InnerText = "Description";
            return listNode;
        }

        private void AppendTableListItem(Commit commit, XmlNode listNode, XmlDocument document)
        {
            XmlElement itemNode = document.CreateElement("item");
            listNode.AppendChild(itemNode);
            XmlElement termNode = document.CreateElement("term");
            itemNode.AppendChild(termNode);
            termNode.InnerText = string.Format("{0} ({1})", commit.Author, commit.Date);
            XmlElement descriptionNode = document.CreateElement("description");
            itemNode.AppendChild(descriptionNode);
            descriptionNode.InnerText = commit.Subject;
        }
    }
}
