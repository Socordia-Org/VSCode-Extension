using Loyc;
using Loyc.Syntax;
using LSP_Server.Core;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace LSP_Server.CompletionScopes;

public class ImportCompletionScope : ContextCompletionHandler
{
    public override Symbol[] MatchingSymbols => new[] { CodeSymbols.Import };

    public override IEnumerable<CompletionItem> GetItems(LNode node)
    {
        yield return new CompletionItem() { Label = "System", Kind = CompletionItemKind.Module };
    }
}