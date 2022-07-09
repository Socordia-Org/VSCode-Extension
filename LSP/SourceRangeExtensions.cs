using Loyc.Syntax;

namespace LSP_Server
{
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
    }
}