using Loyc;
using Loyc.Syntax;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace LSP_Server
{
    public class RootCompletionScope : ContextCompletionHandler
    {
        public override Symbol[] MatchingSymbols => Array.Empty<Symbol>();

        public override IEnumerable<CompletionItem> GetItems(LNode node)
        {
            yield return new CompletionItem() { Label = "using", Kind = CompletionItemKind.Keyword };
            yield return new CompletionItem() { Label = "implement", Kind = CompletionItemKind.Keyword };
            yield return new CompletionItem() { Label = "func", Kind = CompletionItemKind.Keyword };
        }
    }
}