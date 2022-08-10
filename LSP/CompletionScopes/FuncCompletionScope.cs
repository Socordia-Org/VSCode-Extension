using Loyc;
using Loyc.Syntax;
using LSP_Server.Core;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace LSP_Server.CompletionScopes;

public class FuncCompletionScope : ContextCompletionHandler
{
    public override Symbol[] MatchingSymbols => new[] { CodeSymbols.Fn };

    public override IEnumerable<CompletionItem> GetItems(LNode node)
    {
        yield return new CompletionItem() { Label = "let", Kind = CompletionItemKind.Keyword };
        yield return new CompletionItem() { Label = "if", Kind = CompletionItemKind.Keyword };
        yield return new CompletionItem() { Label = "else", Kind = CompletionItemKind.Keyword };
        yield return new CompletionItem() { Label = "inline", Kind = CompletionItemKind.Keyword };
        yield return new CompletionItem() { Label = "while", Kind = CompletionItemKind.Keyword };
        yield return new CompletionItem() { Label = "for", Kind = CompletionItemKind.Keyword };
    }
}