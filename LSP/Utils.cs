using Backlang.Core.CompilerService;
using Furesoft.Core.CodeDom.Compiler.Core.Names;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Socordia.LSP.Core;

namespace Socordia.LSP;

public static class Utils
{
    public static QualifiedName QualifyNamespace(string @namespace)
    {
        var spl = @namespace.Split('.');

        QualifiedName? name = null;

        foreach (var path in spl)
        {
            if (name == null)
            {
                name = new SimpleName(path).Qualify();
                continue;
            }

            name = new SimpleName(path).Qualify(name.Value);
        }

        return name.Value;
    }

    public static IEnumerable<CompletionItem> SuggestPrimitiveTypenames()
    {
        yield return new CompletionItem { Label = "i8", Kind = CompletionItemKind.TypeParameter };
        yield return new CompletionItem { Label = "u8", Kind = CompletionItemKind.TypeParameter };

        yield return new CompletionItem { Label = "i16", Kind = CompletionItemKind.TypeParameter };
        yield return new CompletionItem { Label = "u16", Kind = CompletionItemKind.TypeParameter };

        yield return new CompletionItem { Label = "i32", Kind = CompletionItemKind.TypeParameter };
        yield return new CompletionItem { Label = "u32", Kind = CompletionItemKind.TypeParameter };

        yield return new CompletionItem { Label = "i64", Kind = CompletionItemKind.TypeParameter };
        yield return new CompletionItem { Label = "u64", Kind = CompletionItemKind.TypeParameter };

        yield return new CompletionItem { Label = "f16", Kind = CompletionItemKind.TypeParameter };
        yield return new CompletionItem { Label = "f32", Kind = CompletionItemKind.TypeParameter };
        yield return new CompletionItem { Label = "f64", Kind = CompletionItemKind.TypeParameter };

        yield return new CompletionItem { Label = "char", Kind = CompletionItemKind.TypeParameter };
        yield return new CompletionItem { Label = "string", Kind = CompletionItemKind.TypeParameter };
    }

    public static IEnumerable<CompletionItem> SuggestNamespace(QualifiedName requestedName)
    {
        var namespaceMap = NamespaceMap.From(typeof(int).Assembly,
            typeof(MacroLibAttribute).Assembly,
            typeof(List<>).Assembly);

        var completions = namespaceMap.Resolve(requestedName);

        return completions
            .Select(_ => new CompletionItem { Label = _, Kind = CompletionItemKind.Module });
    }
}