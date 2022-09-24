using Backlang.Driver;
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

        var qualifiedNs = Utils.QualifyNamespace("System.Collections.Generic");
        var requestedName = ConversionUtils.GetQualifiedName(namespaceNode);

        if (namespaceNode.Calls(CodeSymbols.Dot))
        {
            for (int i = 0; i < requestedName.PathLength; i++)
            {
                if (requestedName[i].ToString() == "#error")
                {
                    yield return new CompletionItem() { Label = qualifiedNs[i].ToString(), Kind = CompletionItemKind.Module };
                    break;
                }

                if (qualifiedNs[i].ToString() != requestedName[i].ToString()) break;
            }

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