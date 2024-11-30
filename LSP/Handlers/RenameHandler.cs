using LSP_Server.Core;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace LSP_Server.Handlers;

public class RenameHandler(Workspace workspace, BufferManager bufferManager) : RenameHandlerBase
{
    public override Task<WorkspaceEdit?> Handle(RenameParams request, CancellationToken cancellationToken)
    {
        var identifier = workspace.GetIdentifierAt(request.TextDocument.Uri, request.Position);

        if (identifier == null)
            return Task.FromResult(null as WorkspaceEdit);

        var newText = request.NewName;
        var workspaceEdits = new Dictionary<DocumentUri, IEnumerable<TextEdit>>();

        foreach (var buffer in bufferManager.GetBuffers())
        {
            var refs = workspace.FindReferencesTo(request.TextDocument.Uri, identifier);

            var edits = refs.Select(o => new TextEdit
            {
                NewText = newText,
                Range = o.ToRange()
            }).ToArray();

            workspaceEdits.Add(buffer.Range.Source.FileName, edits);
        }

        return Task.FromResult<WorkspaceEdit?>(new WorkspaceEdit
        {
            Changes = workspaceEdits
        });
    }

    protected override RenameRegistrationOptions CreateRegistrationOptions(RenameCapability capability,
        ClientCapabilities clientCapabilities)
    {
        return new RenameRegistrationOptions { DocumentSelector = TextDocumentSyncHandler.DocumentSelector };
    }
}