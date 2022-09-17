using Loyc.Syntax;
using OmniSharp.Extensions.LanguageServer.Protocol;
using System.Collections.Concurrent;

namespace LSP_Server;

public class BufferManager
{
    private readonly ConcurrentDictionary<DocumentUri, LNode> _buffers = new();

    public LNode GetBuffer(DocumentUri documentPath)
    {
        if (!_buffers.TryGetValue(documentPath, out var buffer))
        {
            return LNode.Missing;
        }

        return buffer;
    }

    public void AddOrUpdateBuffer(DocumentUri documentPath, LNode buffer)
    {
        _buffers.AddOrUpdate(documentPath, buffer, (k, v) => buffer);
    }

    public void Remove(DocumentUri documentPath)
    {
        _buffers.Remove(documentPath, out var _);
    }

    public IEnumerable<LNode> GetBuffers()
    {
        return _buffers.Values;
    }
}