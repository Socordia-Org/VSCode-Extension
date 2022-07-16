using Loyc;
using Loyc.Syntax;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace LSP_Server
{
    public class FuncCompletionScope : ContextCompletionHandler
    {
        public override Symbol[] MatchingSymbols => new[] { CodeSymbols.Fn };

        public override IEnumerable<CompletionItem> GetItems(LNode node)
        {
            yield return new CompletionItem() { Label = "let", Kind = CompletionItemKind.Keyword };
        }
    }
}