using Loyc;
using Loyc.Syntax;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace LSP_Server.CompletionScopes
{
    public class StructCompletionScope : ContextCompletionHandler
    {
        public override Symbol[] MatchingSymbols => new[] { CodeSymbols.Struct };

        public override IEnumerable<CompletionItem> GetItems(LNode node)
        {
            yield return new CompletionItem { Kind = CompletionItemKind.Keyword, Label = "let" };
        }
    }
}