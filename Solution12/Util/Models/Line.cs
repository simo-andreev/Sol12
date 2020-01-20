using System.Numerics;

namespace Solution12.Util.Models
{
    public struct Line
    {
        public readonly Vector2 Start;
        public readonly Vector2 End;

        public Line(Vector2 start, Vector2 end)
        {
            this.Start = start;
            this.End = end;
        }
    }
}