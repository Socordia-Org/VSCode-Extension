using Backlang.Codeanalysis.Parsing.AST;
using Loyc;
using Loyc.Syntax;
using LSP_Server.Core;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace LSP_Server.CompletionScopes;

public class ImplementCompletionScope : ContextCompletionHandler
{
    public override Symbol[] MatchingSymbols => new[] { Symbols.Implementation };

    public override IEnumerable<CompletionItem> GetItems(LNode node)
    {
        yield return new CompletionItem() { Label = "func", Kind = CompletionItemKind.Keyword };
    }
}