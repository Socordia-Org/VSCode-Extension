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

        return Utils.SuggestNamespace(requestedName);
    }
}