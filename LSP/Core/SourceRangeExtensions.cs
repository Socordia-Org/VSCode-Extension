using Loyc.Syntax;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace LSP_Server.Core;

public static class SourceRangeExtensions
{
    public static bool Contains(this SourceRange range, int line, int column)
    {
        var inLeft = false;
        if (range.Start.Line < line) inLeft = true;
        else if (range.Start.Line > line) inLeft = false;
        else inLeft = range.Start.Column <= column;

        var inRight = false;
        if (range.End.Line > line) inRight = true;
        else if (range.End.Line < line) inRight = false;
        else inRight = range.End.Column >= column;

        return inLeft && inRight;
    }

    public static Range ToRange(this SourceRange range)
    {
        var start = new Position(range.Start.Line - 1, range.Start.Column - 1);
        var end = new Position(range.End.Line - 1, range.End.Column - 1);

        return new Range(start, end);
    }
}