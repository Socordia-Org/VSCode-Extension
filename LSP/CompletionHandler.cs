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

            var items = new List<CompletionItem>();
            items.Add(new CompletionItem() { Label = "if", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "else", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "let", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "mut", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "for", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "while", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "func", Kind = CompletionItemKind.Keyword });
            items.Add(new CompletionItem() { Label = "class", Kind = CompletionItemKind.Keyword });

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