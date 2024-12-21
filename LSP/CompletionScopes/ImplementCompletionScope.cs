using Backlang.Codeanalysis.Parsing.AST;
using Backlang.Driver;
using Loyc;
using Loyc.Syntax;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Socordia.LSP.Core;

namespace Socordia.LSP.CompletionScopes;

public class ImplementCompletionScope : ContextCompletionHandler
{
    public override Symbol[] MatchingSymbols => [Symbols.Implementation];

    public override IEnumerable<CompletionItem> GetItems(LNode node)
    {
        if (node[0] is (_, ("'to_expand'", _)))
            foreach (var item in Utils.SuggestPrimitiveTypenames())
                yield return item;
        else
            yield return new CompletionItem { Label = "func", Kind = CompletionItemKind.Keyword };
    }
}