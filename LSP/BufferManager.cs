using Loyc.Syntax;
using System.Collections.Concurrent;

namespace LSP_Server;

public class BufferManager
{
    private readonly ConcurrentDictionary<string, LNode> _buffers = new();

    public LNode GetBuffer(string documentPath)
    {
        if (!_buffers.TryGetValue(documentPath, out var buffer))
        {
            return LNode.Missing;
        }

        return buffer;
    }

    public void AddOrUpdateBuffer(string documentPath, LNode buffer)
    {
        _buffers.AddOrUpdate(documentPath, buffer, (k, v) => buffer);
    }

    public void Remove(string documentPath)
    {
        _buffers.Remove(documentPath, out var _);
    }
}