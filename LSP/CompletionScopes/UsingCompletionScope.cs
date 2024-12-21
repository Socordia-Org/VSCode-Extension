using Loyc;
using Loyc.Syntax;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Socordia.LSP.Core;

namespace Socordia.LSP.CompletionScopes;

public class UsingCompletionScope : ContextCompletionHandler
{
    public override Symbol[] MatchingSymbols => [CodeSymbols.UsingStmt];

    public override IEnumerable<CompletionItem> GetItems(LNode node)
    {
        if (node.Args.Count == 1 && !node.Attrs.Contains(LNode.Id(CodeSymbols.As)))
            yield return new CompletionItem { Label = "as", Kind = CompletionItemKind.Keyword };
    }
}