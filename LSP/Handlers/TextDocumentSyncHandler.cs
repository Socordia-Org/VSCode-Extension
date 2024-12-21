using Backlang.Codeanalysis.Parsing;
using Backlang.Codeanalysis.Parsing.AST;
using Backlang.Contracts;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using Socordia.LSP.Core;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Socordia.LSP.Handlers;

internal class TextDocumentSyncHandler(
    BufferManager bufferManager,
    ILanguageServerFacade protocolProxy,
    Workspace workspace)
    : ITextDocumentSyncHandler
{
    public static readonly TextDocumentSelector DocumentSelector = new(
        new TextDocumentFilter
        {
            Pattern = "**/*.back"
        }
    );

    public TextDocumentSyncKind Change { get; } = TextDocumentSyncKind.Full;


    public TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri)
    {
        return new TextDocumentAttributes(uri, "back");
    }

    public Task<Unit> Handle(DidChangeTextDocumentParams request, CancellationToken cancellationToken)
    {
        var documentPath = request.TextDocument.Uri.GetFileSystemPath();
        var text = request.ContentChanges.FirstOrDefault()?.Text;
        var cu = ParseDocument(documentPath, text);

        var context = new CompilerContext();
        SemanticChecker.Do(cu, context);

        context.Messages.AddRange(cu.Messages);

        bufferManager.AddOrUpdateBuffer(request.TextDocument.Uri, SyntaxTree.Factory.AltList(cu.Body));

        var diagnostics = new List<Diagnostic>();

        foreach (var msg in context.Messages)
            diagnostics.Add(new Diagnostic
            {
                Message = msg.Text,
                CodeDescription = new CodeDescription { Href = request.TextDocument.Uri.ToUri() },
                Source = request.TextDocument.Uri.Path,
                Range = new Range(msg.Range.Start.Line - 1, msg.Range.Start.Column - 1,
                    msg.Range.End.Line - 1, msg.Range.End.Column - 1),
                Severity = msg.Severity == MessageSeverity.Warning
                    ? DiagnosticSeverity.Warning
                    : DiagnosticSeverity.Error
            });

        protocolProxy.TextDocument.PublishDiagnostics(new PublishDiagnosticsParams
        {
            Diagnostics = diagnostics,
            Uri = request.TextDocument.Uri
        });

        return Unit.Task;
    }

    public Task<Unit> Handle(DidOpenTextDocumentParams request, CancellationToken cancellationToken)
    {
        var cu = ParseDocument(request.TextDocument.Uri.GetFileSystemPath(),
            File.ReadAllText(request.TextDocument.Uri.GetFileSystemPath()));
        bufferManager.AddOrUpdateBuffer(request.TextDocument.Uri, SyntaxTree.Factory.AltList(cu.Body));

        workspace.OpenFolder();
        var proj = workspace.GetProjectFile();

        return Unit.Task;
    }

    public Task<Unit> Handle(DidCloseTextDocumentParams request, CancellationToken cancellationToken)
    {
        bufferManager.Remove(request.TextDocument.Uri.GetFileSystemPath());

        return Unit.Task;
    }

    public Task<Unit> Handle(DidSaveTextDocumentParams request, CancellationToken cancellationToken)
    {
        return Unit.Task;
    }

    private static CompilationUnit ParseDocument(string documentPath, string? text)
    {
        return Parser.Parse(new SourceDocument(documentPath, text));
    }

    TextDocumentChangeRegistrationOptions IRegistration<TextDocumentChangeRegistrationOptions, TextSynchronizationCapability>.GetRegistrationOptions(TextSynchronizationCapability capability,
        ClientCapabilities clientCapabilities)
    {
        return new TextDocumentChangeRegistrationOptions
        {
            DocumentSelector = DocumentSelector,
            SyncKind = Change
        };
    }

    TextDocumentOpenRegistrationOptions IRegistration<TextDocumentOpenRegistrationOptions, TextSynchronizationCapability>.GetRegistrationOptions(TextSynchronizationCapability capability,
        ClientCapabilities clientCapabilities)
    {
        return new TextDocumentOpenRegistrationOptions
        {
            DocumentSelector = DocumentSelector
        };
    }

    TextDocumentCloseRegistrationOptions IRegistration<TextDocumentCloseRegistrationOptions, TextSynchronizationCapability>.GetRegistrationOptions(TextSynchronizationCapability capability,
        ClientCapabilities clientCapabilities)
    {
        return new TextDocumentCloseRegistrationOptions
        {
            DocumentSelector = DocumentSelector
        };
    }

    TextDocumentSaveRegistrationOptions IRegistration<TextDocumentSaveRegistrationOptions, TextSynchronizationCapability>.GetRegistrationOptions(TextSynchronizationCapability capability,
        ClientCapabilities clientCapabilities)
    {
        return new TextDocumentSaveRegistrationOptions
        {
            DocumentSelector = DocumentSelector
        };
    }
}