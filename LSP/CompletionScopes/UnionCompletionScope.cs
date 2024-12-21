using Loyc;
using Loyc.Syntax;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Socordia.LSP.Core;

namespace Socordia.LSP.CompletionScopes;

public class UnionCompletionScope : ContextCompletionHandler
{
    public override Symbol[] MatchingSymbols => [CodeSymbols.Struct];

    public override IEnumerable<CompletionItem> GetItems(LNode node)
    {
        yield return new CompletionItem { Kind = CompletionItemKind.Keyword, Label = "let" };
    }
}