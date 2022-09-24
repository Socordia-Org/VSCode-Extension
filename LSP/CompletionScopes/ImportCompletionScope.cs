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
        var requestedName = ConversionUtils.GetQualifiedName(namespaceNode);

        var namespaces = new[] {
            "System.Collections.Generic",
            "System",
            "Backlang.Core",
            "System.Diagnostics"
        };

        var namespaceMap = NamespaceMap.From(namespaces);

        var completions = namespaceMap.Resolve(requestedName).ToArray();

        return completions
            .Select(_ => new CompletionItem { Label = _, Kind = CompletionItemKind.Module });
    }
}