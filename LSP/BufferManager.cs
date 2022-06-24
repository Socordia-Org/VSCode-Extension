using System.Collections.Concurrent;

public class BufferManager
{
    private ConcurrentDictionary<string, string> _buffers = new();

    public string GetBuffer(string documentPath)
    {
        return _buffers.TryGetValue(documentPath, out var buffer) ? buffer : null;
    }

    public void UpdateBuffer(string documentPath, string buffer)
    {
        _buffers.AddOrUpdate(documentPath, buffer, (k, v) => buffer);
    }
}