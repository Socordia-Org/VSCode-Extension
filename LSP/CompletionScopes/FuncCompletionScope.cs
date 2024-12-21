using Loyc;
using Loyc.Syntax;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Socordia.LSP.Core;

namespace Socordia.LSP.CompletionScopes;

public class FuncCompletionScope : ContextCompletionHandler
{
    public override Symbol[] MatchingSymbols => [CodeSymbols.Fn];

    public override IEnumerable<CompletionItem> GetItems(LNode node)
    {
        yield return new CompletionItem { Label = "let", Kind = CompletionItemKind.Keyword };
        yield return new CompletionItem { Label = "if", Kind = CompletionItemKind.Keyword };
        yield return new CompletionItem { Label = "else", Kind = CompletionItemKind.Keyword };
        yield return new CompletionItem { Label = "inline", Kind = CompletionItemKind.Keyword };
        yield return new CompletionItem { Label = "while", Kind = CompletionItemKind.Keyword };
        yield return new CompletionItem { Label = "for", Kind = CompletionItemKind.Keyword };
        yield return new CompletionItem { Label = "do", Kind = CompletionItemKind.Keyword };

        yield return new CompletionItem { Label = "print", Kind = CompletionItemKind.Function };
    }
}