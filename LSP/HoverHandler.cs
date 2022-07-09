using Backlang.Codeanalysis.Parsing;
using Loyc.Syntax;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System.Text;

namespace LSP_Server
{
    public class HoverHandler : IHoverHandler
    {
        private readonly ILanguageServerFacade protocolProxy;
        private readonly BufferManager _bufferManager;
        private SynchronizationCapability _capability;

        public HoverHandler(BufferManager bufferManager, ILanguageServerFacade protocolProxy)
        {
            _bufferManager = bufferManager;
            this.protocolProxy = protocolProxy;
        }

        public HoverRegistrationOptions GetRegistrationOptions(HoverCapability capability, ClientCapabilities clientCapabilities)
        {
            return new HoverRegistrationOptions
            {
                DocumentSelector = new DocumentSelector(
                    new DocumentFilter()
                    {
                        Pattern = "**/*.back"
                    }
                )
            };
        }

        public async Task<Hover> Handle(HoverParams request, CancellationToken token)
        {
            var documentPath = request.TextDocument.Uri.ToString();
            var text = _bufferManager.GetBuffer(documentPath);

            var filebody = Encoding.Default.GetBytes(text);
            var document = new SourceFile<StreamCharSource>(new(new MemoryStream(filebody)), documentPath);
            SyntaxTree.Factory = new(document);
            var result = Parser.Parse(document);

            LNode matchingNode = LNode.Missing;

            foreach (var node in result.Tree)
            {
                node.ReplaceRecursive((_) =>
                {
                    if (node.Range.Contains(request.Position.Line, request.Position.Character))
                    {
                        matchingNode = node;
                    }

                    return _;
                }, LNode.ReplaceOpt.ReplaceRoot);
            }

            if (matchingNode != LNode.Missing)
            {
                return new Hover()
                {
                    Contents =
                        new MarkedStringsOrMarkupContent(new MarkupContent()
                        {
                            Value = matchingNode.Name.Name + ": A Keyword",
                            Kind = MarkupKind.PlainText
                        })
                };
            }

            return null;
        }
    }
}