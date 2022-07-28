using Backlang.Codeanalysis.Parsing.AST;
using Backlang.Driver;
using Backlang.Driver.Compiling.Targets.Dotnet;
using Loyc;
using Loyc.Syntax;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace LSP_Server;

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
        if (node.ArgCount == 0)
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
        else
        {
            if (node.ArgCount == 2 && node.Args[1].Calls(Symbols.Block))
            {
                var target = plugins.Targets
                                .FirstOrDefault(_ => _.HasIntrinsics
                                    && _.Name == node.Args[0].Name.Name);

                if (target != null)
                {
                    var intrinsicNames = GetAvailableIntrinsicNames(target.IntrinsicType);

                    foreach (var name in intrinsicNames)
                    {
                        yield return new CompletionItem() { Label = name, Kind = CompletionItemKind.Method };
                    }
                }
            }
        }
    }

    private static string[] GetAvailableIntrinsicNames(Type intrinsicType)
    {
        return intrinsicType.GetMethods()
            .Where(_ => _.IsStatic)
            .Select(_ => _.Name.ToLower()).Distinct().ToArray();
    }
}