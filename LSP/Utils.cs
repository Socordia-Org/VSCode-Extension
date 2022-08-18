﻿using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace LSP_Server
{
    public static class Utils
    {
        public static IEnumerable<CompletionItem> GetTypes()
        {
            yield return new CompletionItem() { Label = "i8", Kind = CompletionItemKind.TypeParameter };
            yield return new CompletionItem() { Label = "u8", Kind = CompletionItemKind.TypeParameter };

            yield return new CompletionItem() { Label = "i16", Kind = CompletionItemKind.TypeParameter };
            yield return new CompletionItem() { Label = "u16", Kind = CompletionItemKind.TypeParameter };

            yield return new CompletionItem() { Label = "i32", Kind = CompletionItemKind.TypeParameter };
            yield return new CompletionItem() { Label = "u32", Kind = CompletionItemKind.TypeParameter };

            yield return new CompletionItem() { Label = "i64", Kind = CompletionItemKind.TypeParameter };
            yield return new CompletionItem() { Label = "u64", Kind = CompletionItemKind.TypeParameter };

            yield return new CompletionItem() { Label = "f16", Kind = CompletionItemKind.TypeParameter };
            yield return new CompletionItem() { Label = "f32", Kind = CompletionItemKind.TypeParameter };
            yield return new CompletionItem() { Label = "f64", Kind = CompletionItemKind.TypeParameter };

            yield return new CompletionItem() { Label = "char", Kind = CompletionItemKind.TypeParameter };
            yield return new CompletionItem() { Label = "string", Kind = CompletionItemKind.TypeParameter };
        }
    }
}