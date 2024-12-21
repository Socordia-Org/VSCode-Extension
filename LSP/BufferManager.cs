using System.Collections.Concurrent;
using Loyc.Syntax;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace Socordia.LSP;

public class BufferManager
{
    private readonly ConcurrentDictionary<DocumentUri, LNode> _buffers = new();

    public LNode GetBuffer(DocumentUri documentPath)
    {
        return !_buffers.TryGetValue(documentPath, out var buffer) ? LNode.Missing : buffer;
    }

    public void AddOrUpdateBuffer(DocumentUri documentPath, LNode buffer)
    {
        _buffers.AddOrUpdate(documentPath, buffer, (k, v) => buffer);
    }

    public void Remove(DocumentUri documentPath)
    {
        _buffers.Remove(documentPath, out _);
    }

    public IEnumerable<LNode> GetBuffers()
    {
        return _buffers.Values;
    }
}