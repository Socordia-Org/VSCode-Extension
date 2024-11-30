using Backlang.Contracts;
using Loyc;
using Loyc.Syntax;
using LSP_Server.Core;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace LSP_Server.CompletionScopes;

public class InlineCompletionScope(PluginContainer plugins) : ContextCompletionHandler
{
    public override Symbol[] MatchingSymbols => [(Symbol)"inline"];

    public override IEnumerable<CompletionItem> GetItems(LNode node)
    {
        if (node.ArgCount == 0)
        {
            yield return new CompletionItem { Label = "dotnet", Kind = CompletionItemKind.Value };

            var availableTargets = plugins.Targets
                .Where(_ => _.HasIntrinsics)
                .Select(_ => _.Name)
                .ToArray();

            foreach (var target in availableTargets)
                yield return new CompletionItem { Label = target, Kind = CompletionItemKind.Value };
        }
        else
        {
            if (node.ArgCount == 2 && node.Args[1].Calls(CodeSymbols.Braces))
            {
                var target = plugins.Targets
                    .FirstOrDefault(_ => _.HasIntrinsics
                                         && _.Name == node.Args[0].Name.Name);

                if (target != null)
                {
                    var intrinsicNames = GetAvailableIntrinsicNames(target.IntrinsicType);

                    foreach (var name in intrinsicNames)
                        yield return new CompletionItem { Label = name, Kind = CompletionItemKind.Method };

                    var constants = GetAllConstants(target.IntrinsicType);
                    foreach (var constant in constants)
                        yield return new CompletionItem { Label = constant, Kind = CompletionItemKind.Constant };
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

    private IEnumerable<string> GetAllConstants(Type intrinsicType)
    {
        foreach (var field in intrinsicType.GetFields())
        {
            var names = Enum.GetNames(field.FieldType);

            foreach (var name in names) yield return name;
        }
    }
}