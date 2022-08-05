using Backlang.Codeanalysis.Parsing;
using Loyc.Syntax;
using LSP_Server;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

internal class TextDocumentSyncHandler : ITextDocumentSyncHandler
{
    public static readonly DocumentSelector DocumentSelector = new(
        new DocumentFilter()
        {
            Pattern = "**/*.back"
        }
    );

    private readonly BufferManager _bufferManager;
    private readonly ILanguageServerFacade protocolProxy;

    public TextDocumentSyncHandler(BufferManager bufferManager, ILanguageServerFacade protocolProxy)
    {
        _bufferManager = bufferManager;
        this.protocolProxy = protocolProxy;
    }

    public TextDocumentSyncKind Change { get; } = TextDocumentSyncKind.Full;

    public TextDocumentChangeRegistrationOptions GetRegistrationOptions(SynchronizationCapability capability, ClientCapabilities clientCapabilities)
    {
        return new TextDocumentChangeRegistrationOptions()
        {
            DocumentSelector = DocumentSelector,
            SyncKind = Change,
        };
    }

    TextDocumentOpenRegistrationOptions IRegistration<TextDocumentOpenRegistrationOptions, SynchronizationCapability>.GetRegistrationOptions(SynchronizationCapability capability, ClientCapabilities clientCapabilities)
    {
        return new TextDocumentOpenRegistrationOptions() { DocumentSelector = DocumentSelector };
    }

    TextDocumentCloseRegistrationOptions IRegistration<TextDocumentCloseRegistrationOptions, SynchronizationCapability>.GetRegistrationOptions(SynchronizationCapability capability, ClientCapabilities clientCapabilities)
    {
        return new TextDocumentCloseRegistrationOptions() { DocumentSelector = DocumentSelector };
    }

    TextDocumentSaveRegistrationOptions IRegistration<TextDocumentSaveRegistrationOptions, SynchronizationCapability>.GetRegistrationOptions(SynchronizationCapability capability, ClientCapabilities clientCapabilities)
    {
        return null;
    }

    public TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri)
    {
        return new TextDocumentAttributes(uri, "back");
    }

    public Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken)
    {
        var documentPath = request.TextDocument.Uri.ToString();
        var text = request.ContentChanges.FirstOrDefault()?.Text;
        (LNodeList Tree, List<Message> Messages) result = ParseDocument(documentPath, text);

        _bufferManager.AddOrUpdateBuffer(documentPath, SyntaxTree.Factory.AltList(result.Tree));

        var diagnostics = new List<Diagnostic>();

        foreach (var msg in result.Messages)
        {
            diagnostics.Add(new Diagnostic()
            {
                Message = msg.Text,
                CodeDescription = new CodeDescription() { Href = request.TextDocument.Uri.ToUri() },
                Source = request.TextDocument.Uri.Path,
                Range = new Range(msg.Range.Start.Line - 1, msg.Range.Start.Column - 1,
                msg.Range.End.Line - 1, msg.Range.End.Column - 1)
            });
        }

        protocolProxy.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams()
        {
            Diagnostics = diagnostics,
            Uri = request.TextDocument.Uri
        });

        return Unit.Task;
    }

    public Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
    {
        var tree = ParseDocument(request.TextDocument.Uri.ToString(), File.ReadAllText(request.TextDocument.Uri.ToString()));
        _bufferManager.AddOrUpdateBuffer(request.TextDocument.Uri.ToString(), SyntaxTree.Factory.AltList(tree.Tree));

        return Unit.Task;
    }

    public Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
    {
        _bufferManager.Remove(request.TextDocument.Uri.ToString());

        return Unit.Task;
    }

    public Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
    {
        return Unit.Task;
    }

    private static (LNodeList Tree, List<Message> Messages) ParseDocument(string documentPath, string? text)
    {
        return Parser.Parse(new SourceDocument(documentPath, text));
    }
}