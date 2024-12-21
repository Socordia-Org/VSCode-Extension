using Loyc;
using Loyc.Syntax;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Socordia.LSP.Core;

public abstract class ContextCompletionHandler
{
    public abstract Symbol[] MatchingSymbols { get; }

    public abstract IEnumerable<CompletionItem> GetItems(LNode node);
}