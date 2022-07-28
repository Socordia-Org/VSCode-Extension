using Backlang.Driver;
using Loyc.Syntax;
using LSP_Server.Core;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace LSP_Server;

public partial class CompletionHandler : ICompletionHandler
{
    private BufferManager _bufferManager;

    private ContextCompletionManager _completionManager;

    public CompletionHandler(BufferManager bufferManager, PluginContainer plugins)
    {
        _bufferManager = bufferManager;

        _completionManager = new ContextCompletionManager
        {
            new ImplementCompletionScope(),
            new FuncCompletionScope(),
            new LetCompletionScope(),
            new UsingCompletionScope(),
            new InlineCompletionScope(plugins),
        };

        _completionManager.RootScope = new RootCompletionScope();
    }

    public CompletionRegistrationOptions GetRegistrationOptions(CompletionCapability capability, ClientCapabilities clientCapabilities)
    {
        return new CompletionRegistrationOptions
        {
            DocumentSelector = TextDocumentSyncHandler.DocumentSelector,
            WorkDoneProgress = false,
            TriggerCharacters = new[] { " ", ".", "(" },
        };
    }

    public Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
    {
        var documentPath = request.TextDocument.Uri.ToString();
        var buffer = _bufferManager.GetBuffer(documentPath);

        var matchings = GetMatchingNodesInRange(request, buffer);

        var completions = new CompletionList(_completionManager.GetItems(matchings));

        return Task.FromResult(completions);
    }

    private static LNodeList GetMatchingNodesInRange(CompletionParams request, LNode buffer)
    {
        var matchings = new LNodeList();

        if (buffer == null) return matchings;

        foreach (var node in buffer.Descendants())
        {
            if (node == null || node.Range.Length == 0) continue;

            if (node != null && node.Range.Contains(request.Position.Line + 1, request.Position.Character + 1)) // +1, because zero based index in lsp host
            {
                matchings.Add(node);
            }
        }

        return matchings;
    }
}