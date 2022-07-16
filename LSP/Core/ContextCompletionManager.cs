using Loyc;
using Loyc.Syntax;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Collections;

namespace LSP_Server.Core
{
    public class ContextCompletionManager : IEnumerable<ContextCompletionHandler>
    {
        private List<ContextCompletionHandler> _handlers = new();

        public RootCompletionScope RootScope { get; set; }

        public IEnumerator<ContextCompletionHandler> GetEnumerator()
        {
            return _handlers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _handlers.GetEnumerator();
        }

        public void Add(ContextCompletionHandler handler)
        {
            _handlers.Add(handler);
        }

        public IEnumerable<CompletionItem> GetItems(LNodeList matchings)
        {
            if (matchings.IsEmpty) // root scope
            {
                return RootScope.GetItems(LNode.Missing);
            }

            var items = new List<CompletionItem>();

            foreach (var matchingNode in matchings)
            {
                var scope = GetMatchingScope(matchingNode);

                if (scope == null) continue;

                items = new(scope.GetItems(matchingNode));
            }

            return items;
        }

        private static bool MatchCall(Symbol[] matchingSymbols, LNode matchingNode)
        {
            bool matched = false;

            foreach (var symbol in matchingSymbols)
            {
                matched |= matchingNode.Calls(symbol);
            }

            return matched;
        }

        private ContextCompletionHandler GetMatchingScope(LNode matchingNode)
        {
            foreach (var handler in _handlers)
            {
                if (MatchCall(handler.MatchingSymbols, matchingNode))
                {
                    return handler;
                }
            }

            return null;
        }
    }
}