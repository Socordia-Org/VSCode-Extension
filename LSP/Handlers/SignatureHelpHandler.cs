using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Socordia.LSP.Handlers;

public class SignatureHelpHandler : SignatureHelpHandlerBase
{
    public override Task<SignatureHelp?> Handle(SignatureHelpParams request, CancellationToken cancellationToken)
    {
        var para = new List<ParameterInformation>
        {
            new() { Documentation = "The message", Label = "msg" }
        };

        var sigs = new SignatureInformation
        {
            Label = "print(msg : string)",
            ActiveParameter = 0,
            Parameters = para,
            Documentation = "Print something to the console"
        };
        var sigi = new SignatureInformation
        {
            Label = "print(msg : i32)",
            ActiveParameter = 0,
            Parameters = para,
            Documentation = "Print something to the console"
        };
        var sigo = new SignatureInformation
        {
            Label = "print(msg : obj)",
            ActiveParameter = 0,
            Parameters = para,
            Documentation = "Print something to the console"
        };

        var sigHelp = new SignatureHelp
        {
            ActiveParameter = 0,
            ActiveSignature = 0,
            Signatures = Container.From(sigs, sigi, sigo)
        };

        return Task.FromResult(sigHelp);
    }

    protected override SignatureHelpRegistrationOptions CreateRegistrationOptions(SignatureHelpCapability capability,
        ClientCapabilities clientCapabilities)
    {
        return new SignatureHelpRegistrationOptions
            { DocumentSelector = TextDocumentSyncHandler.DocumentSelector, TriggerCharacters = Container.From("(") };
    }
}