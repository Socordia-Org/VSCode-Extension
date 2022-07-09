using Loyc.Syntax;
using System.Collections.Concurrent;

public class BufferManager
{
    private ConcurrentDictionary<string, LNode> _buffers = new();

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
}