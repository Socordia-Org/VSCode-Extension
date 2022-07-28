using Backlang.Driver;
using Loyc;
using Loyc.Syntax;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace LSP_Server
{
    public class InlineCompletionScope : ContextCompletionHandler
    {
        private PluginContainer plugins;

        public InlineCompletionScope(PluginContainer plugins)
        {
            this.plugins = plugins;
        }

        public override Symbol[] MatchingSymbols => new[] { (Symbol)"inline" };

        public override IEnumerable<CompletionItem> GetItems(LNode node)
        {
            //ToDo: find why inline has no range
            if (node.ArgCount == 1)
            {
                yield return new CompletionItem() { Label = "dotnet", Kind = CompletionItemKind.Value };

                var availableTargets = plugins.Targets
                    .Where(_ => _.HasIntrinsics)
                    .Select(_ => _.Name)
                    .ToArray();

                foreach (var target in availableTargets)
                {
                    yield return new CompletionItem() { Label = target, Kind = CompletionItemKind.Value };
                }
            }
        }
    }
}