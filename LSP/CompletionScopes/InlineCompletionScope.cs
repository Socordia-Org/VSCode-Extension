using Loyc;
using Loyc.Syntax;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace LSP_Server
{
    public class InlineCompletionScope : ContextCompletionHandler
    {
        public override Symbol[] MatchingSymbols => new[] { (Symbol)"inline" };

        public override IEnumerable<CompletionItem> GetItems(LNode node)
        {
            //ToDo: find why inline has no range
            if (node.ArgCount == 1)
            {
                yield return new CompletionItem() { Label = "dotnet", Kind = CompletionItemKind.Value };
                yield return new CompletionItem() { Label = "bs2k", Kind = CompletionItemKind.Value };
            }
        }
    }
}