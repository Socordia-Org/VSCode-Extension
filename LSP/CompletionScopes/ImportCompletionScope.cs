using Backlang.Driver;
using Loyc;
using Loyc.Syntax;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Socordia.LSP.Core;

namespace Socordia.LSP.CompletionScopes;

public class ImportCompletionScope : ContextCompletionHandler
{
    public override Symbol[] MatchingSymbols => [CodeSymbols.Import];

    public override IEnumerable<CompletionItem> GetItems(LNode node)
    {
        var namespaceNode = node[0];
        var requestedName = ConversionUtils.GetQualifiedName(namespaceNode);

        return Utils.SuggestNamespace(requestedName);
    }
}