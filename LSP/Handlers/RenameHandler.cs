using LSP_Server.Core;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace LSP_Server.Handlers
{
    public class RenameHandler : RenameHandlerBase
    {
        public RenameHandler(Workspace workspace)
        {
            Workspace = workspace;
        }

        public Workspace Workspace { get; set; }

        public override Task<WorkspaceEdit?> Handle(RenameParams request, CancellationToken cancellationToken)
        {
            var identifier = Workspace.GetIdentifierAt(request.TextDocument.Uri, request.Position);

            if (identifier == null)
                return Task.FromResult(null as WorkspaceEdit);

            var newText = request.NewName;
            var refs = Workspace.FindReferencesTo(request.TextDocument.Uri, identifier);

            var edits = refs.Select(o => new TextEdit
            {
                NewText = newText,
                Range = o.ToRange()
            }).ToArray();

            return Task.FromResult<WorkspaceEdit?>(new WorkspaceEdit
            {
                Changes = new Dictionary<DocumentUri, IEnumerable<TextEdit>>
                {
                    { request.TextDocument.Uri, edits }
                }
            });
        }

        protected override RenameRegistrationOptions CreateRegistrationOptions(RenameCapability capability, ClientCapabilities clientCapabilities)
        {
            return new RenameRegistrationOptions { DocumentSelector = TextDocumentSyncHandler.DocumentSelector };
        }
    }
}