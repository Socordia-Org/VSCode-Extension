using Loyc.Syntax;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Socordia.LSP.Core;

namespace Socordia.LSP.Handlers;

public class HoverHandler(BufferManager bufferManager, ILanguageServerFacade protocolProxy)
    : IHoverHandler
{
    private readonly ILanguageServerFacade protocolProxy = protocolProxy;

    public HoverRegistrationOptions GetRegistrationOptions(HoverCapability capability,
        ClientCapabilities clientCapabilities)
    {
        return new HoverRegistrationOptions
        {
            DocumentSelector = TextDocumentSyncHandler.DocumentSelector,
            WorkDoneProgress = false
        };
    }

    public Task<Hover?> Handle(HoverParams request, CancellationToken token)
    {
        var documentPath = request.TextDocument.Uri.ToString();
        var buffer = bufferManager.GetBuffer(documentPath);

        LNode matchingNode = LNode.Missing;

        foreach (var node in buffer.Descendants())
        {
            if (node.Range.Length == 0 || node == null) continue;

            if (node.Range.Contains(request.Position.Line + 1, request.Position.Character + 1)) matchingNode = node;
        }

        if (matchingNode != LNode.Missing)
        {
            var content = "";

            if (matchingNode.ArgCount == 1 && matchingNode.Args[0].HasValue)
                content = matchingNode.Name.Name;
            else
                content = matchingNode.ToString();

            return Task.FromResult<Hover?>(new Hover
            {
                Contents =
                    new MarkedStringsOrMarkupContent(new MarkupContent
                    {
                        Value = matchingNode.Name.Name + ": " + content,
                        Kind = MarkupKind.PlainText
                    })
            });
        }

        return Task.FromResult<Hover?>(null);
    }
}