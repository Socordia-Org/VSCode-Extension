﻿using Backlang.Codeanalysis.Parsing;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace LSP_Server
{
    public class CompletionHandler : ICompletionHandler
    {
        private BufferManager _bufferManager;

        public CompletionHandler(BufferManager bufferManager)
        {
            _bufferManager = bufferManager;
        }

        public CompletionRegistrationOptions GetRegistrationOptions(CompletionCapability capability, ClientCapabilities clientCapabilities)
        {
            return new CompletionRegistrationOptions
            {
                DocumentSelector = new DocumentSelector(
                    new DocumentFilter()
                    {
                        Pattern = "**/*.back"
                    }
                ),
                ResolveProvider = false
            };
        }

        public async Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
        {
            var documentPath = request.TextDocument.Uri.ToString();
            var buffer = _bufferManager.GetBuffer(documentPath);

            var result = Parser.Parse(new SourceDocument(request.TextDocument.Uri.Path, buffer));

            var items = new List<CompletionItem>();
            items.Add(new CompletionItem() { Label = "module", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "using", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "as", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "if", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "else", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "let", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "mut", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "const", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "global", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "for", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "in", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "of", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "while", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "func", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "class", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "struct", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "enum", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "interface", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "try", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "catch", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "finally", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "implement", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "with", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "match", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "finally", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "bitfield", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "default", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "sizeof", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "type", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "switch", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "case", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "break", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "continue", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "return", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "when", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "public", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "protected", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "private", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "static", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "extern", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "abstract", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "override", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "operator", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "true", Kind = CompletionItemKind.Value });
            items.Add(new CompletionItem() { Label = "false", Kind = CompletionItemKind.Value });
            items.Add(new CompletionItem() { Label = "none", Kind = CompletionItemKind.Value });

            return new CompletionList(items);
        }

        private static int GetPosition(string buffer, int line, int col)
        {
            var position = 0;
            for (var i = 0; i < line; i++)
            {
                position = buffer.IndexOf('\n', position) + 1;
            }
            return position + col;
        }
    }
}