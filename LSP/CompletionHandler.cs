using Backlang.Codeanalysis.Parsing.AST;
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

            var matchings = new LNodeList();

            foreach (var node in buffer?.Descendants())
            {
                if (node == null || node.Range.Length == 0) continue;

                if (node != null && node.Range.Contains(request.Position.Line + 1, request.Position.Character + 1))
                {
                    matchings.Add(node);
                }
            }

            if (matchings.IsEmpty) // root scope
            {
                items.Add(new CompletionItem() { Label = "using", Kind = CompletionItemKind.Keyword });
                items.Add(new CompletionItem() { Label = "implement", Kind = CompletionItemKind.Keyword });
                items.Add(new CompletionItem() { Label = "func", Kind = CompletionItemKind.Keyword });
            }

            foreach (var matchingNode in matchings)
            {
                if (matchingNode.Calls(CodeSymbols.UsingStmt))
                {
                    items.Clear();

                    if (matchingNode.Args.Count == 1 && !matchingNode.Attrs.Contains(LNode.Id(CodeSymbols.As)))
                    {
                        items.Add(new CompletionItem() { Label = "as", Kind = CompletionItemKind.Keyword });
                    }
                }
                if (matchingNode.Calls(CodeSymbols.Var))
                {
                    items.Clear();

                    if (!matchingNode.Attrs.Contains(LNode.Id(Symbols.Mutable)))
                    {
                        items.Add(new CompletionItem() { Label = "mut", Kind = CompletionItemKind.Keyword });
                    }
                }
                else if (matchingNode.Calls(CodeSymbols.Fn))
                {
                    items.Clear();

                    items.Add(new CompletionItem() { Label = "let", Kind = CompletionItemKind.Keyword });
                }
                else if (matchingNode.Calls(Symbols.Implementation))
                {
                    items.Clear();

                    items.Add(new CompletionItem() { Label = "func", Kind = CompletionItemKind.Keyword });
                }
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