using System.Collections.Concurrent;

public class BufferManager
{
    private ConcurrentDictionary<string, string> _buffers = new();

    public string GetBuffer(string documentPath)
    {
        return _buffers.TryGetValue(documentPath, out var buffer) ? buffer : File.ReadAllText(documentPath);
    }

    public void UpdateBuffer(string documentPath, string buffer)
    {
        _buffers.AddOrUpdate(documentPath, buffer, (k, v) => buffer);
    }
}