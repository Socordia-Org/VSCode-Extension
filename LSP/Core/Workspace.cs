using Backlang.Codeanalysis.Parsing;
using Loyc.Syntax;
using Microsoft.Build.Construction;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace LSP_Server.Core;

public class Workspace(BufferManager bufferManager)
{
    public void OpenFolder()
    {
        var fi = new FileInfo(bufferManager.GetBuffers().First().Range.Source.FileName);
        var dir = fi.Directory;
        var files = dir.GetFiles("*.back", SearchOption.AllDirectories).Select(_ => new Uri(_.FullName));

        foreach (var file in files)
        {
            var documentPath =
                new DocumentUri(file.Scheme, file.Authority, file.AbsolutePath, file.Query, file.Fragment);
            var buffer = Parser.Parse(new SourceDocument(documentPath.GetFileSystemPath(),
                File.ReadAllText(documentPath.GetFileSystemPath())));
            bufferManager.AddOrUpdateBuffer(documentPath, SyntaxTree.Factory.AltList(buffer.Body));
        }
    }

    public ProjectRootElement GetProjectFile()
    {
        var fi = new FileInfo(bufferManager.GetBuffers().First().Range.Source.FileName);
        var dir = fi.Directory;
        var files = dir.GetFiles("*.backproj", SearchOption.AllDirectories).First();

        return ProjectRootElement.Open(files.FullName);
    }

    public LNode GetIdentifierAt(DocumentUri uri, Position position)
    {
        var buffer = bufferManager.GetBuffer(uri);

        foreach (var node in buffer.Descendants())
        {
            if (node.Range.Length == 0 || node == null) continue;

            if (node.Range.Contains(position.Line + 1, position.Character) && node.IsId) return node;
        }

        return LNode.Missing;
    }

    public IEnumerable<SourceRange> FindReferencesTo(DocumentUri uri, LNode identifier)
    {
        foreach (var tree in bufferManager.GetBuffers())
        {
            var documentPath = uri.ToString();

            LNode matchingNode = LNode.Missing;

            foreach (var node in tree.Descendants())
            {
                if (node.Range.Length == 0 || node == null) continue;

                if (node.IsIdNamed(identifier.Name)) yield return node.Range;
            }
        }
    }
}