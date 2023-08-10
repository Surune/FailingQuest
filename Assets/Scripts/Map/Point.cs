using System;

// Vector2 containing two integers.
namespace Map
{
    public class Point : IEquatable<Point>
    {
        public int x { get; private set; }
        public int y { get; private set; }

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(Point other)
        {
            return x == other.x && y == other.y;
        }
    }
}
