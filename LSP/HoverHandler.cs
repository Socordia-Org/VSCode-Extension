using Loyc.Syntax;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

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
                DocumentSelector = TextDocumentSyncHandler.DocumentSelector
            };
        }

        public async Task<Hover> Handle(HoverParams request, CancellationToken token)
        {
            var documentPath = request.TextDocument.Uri.ToString();
            var buffer = _bufferManager.GetBuffer(documentPath);

            LNode matchingNode = LNode.Missing;

            foreach (var node in buffer.Args)
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