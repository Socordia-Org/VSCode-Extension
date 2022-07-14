using Loyc.Syntax;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Text;

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
                DocumentSelector = TextDocumentSyncHandler.DocumentSelector,
                WorkDoneProgress = true,
                TriggerCharacters = new[] { " ", "." },
                ResolveProvider = false
            };
        }

        public async Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
        {
            var documentPath = request.TextDocument.Uri.ToString();
            var buffer = _bufferManager.GetBuffer(documentPath);

            var items = new List<CompletionItem>();

            LNode matchingNode = LNode.Missing;

            foreach (var node in buffer?.Descendants())
            {
                if (node != null && node.Range.Contains(request.Position.Line + 1, request.Position.Character + 1))
                {
                    matchingNode = node;
                }
            }

            if (matchingNode.Calls(CodeSymbols.Fn) || matchingNode.Calls("'{}"))
            {
                items.Add(new CompletionItem() { Label = "let", Kind = CompletionItemKind.Keyword });
            }
            else if (matchingNode.Calls(CodeSymbols.Var))
            {
                if (!matchingNode.Attrs.Contains(LNode.Id("#mutable")))
                {
                    items.Add(new CompletionItem() { Label = "mut", Kind = CompletionItemKind.Keyword });
                }
            }
            else if (matchingNode.Calls(CodeSymbols.UsingStmt))
            {
                items.Add(new CompletionItem() { Label = "as", Kind = CompletionItemKind.Keyword });
            }
            else
            {
                items.Add(new CompletionItem() { Label = "using", Kind = CompletionItemKind.Keyword });
            }

            return new CompletionList(items);
        }

        private string? NodeToString(LNode node)
        {
            var sb = new StringBuilder();

            sb.Append(node.Name);
            sb.AppendLine(node.Range.ToString());

            foreach (var n in node.Args)
            {
                sb.AppendLine("\t" + NodeToString(n));
            }

            return sb.ToString();
        }

        private string RangeToString(SourceRange range)
        {
            return $"{range.Start}-{range.End}";
        }
    }
}