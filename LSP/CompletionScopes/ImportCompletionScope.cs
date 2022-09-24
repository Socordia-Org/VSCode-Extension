using Loyc;
using Loyc.Syntax;
using LSP_Server.Core;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace LSP_Server.CompletionScopes;

public class ImportCompletionScope : ContextCompletionHandler
{
    public override Symbol[] MatchingSymbols => new[] { CodeSymbols.Import };

    public override IEnumerable<CompletionItem> GetItems(LNode node)
    {
        LNode namespaceNode = node[0];

        if (namespaceNode.Calls(CodeSymbols.Dot))
        {
            if (namespaceNode[0].IsIdNamed("Backlang"))
            {
                yield return new CompletionItem() { Label = "Core", Kind = CompletionItemKind.Module };
            }
        }
        else
        {
            yield return new CompletionItem() { Label = "System", Kind = CompletionItemKind.Module };
            yield return new CompletionItem() { Label = "Backlang", Kind = CompletionItemKind.Module };
        }
    }
}