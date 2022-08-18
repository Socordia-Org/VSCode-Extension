using Loyc;
using Loyc.Syntax;
using LSP_Server.Core;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace LSP_Server.CompletionScopes;

public class TypeCompletionScope : ContextCompletionHandler
{
    public override Symbol[] MatchingSymbols => new[] { CodeSymbols.Struct, CodeSymbols.Class };

    public override IEnumerable<CompletionItem> GetItems(LNode node)
    {
        yield return new CompletionItem { Kind = CompletionItemKind.Keyword, Label = "let" };
    }
}