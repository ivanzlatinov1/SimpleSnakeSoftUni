using System;
using System.Runtime.CompilerServices;

namespace SimpleSnake.GameObjects
{
    public readonly struct Point : IEquatable<Point>
    {
        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        
        public int X { get; }
        public int Y { get; }
        public bool Equals(Point other)
            => this.X == other.X && this.Y == other.Y;

        public override bool Equals(object obj)
            => obj is Point p && this.Equals(p);

        public override int GetHashCode()
            => HashCode.Combine(this.X, this.Y);

        public static Point operator +(Point a, Point b) => new(a.X + b.X, a.Y + b.Y);
        public static Point operator *(Point a, int k) => k * a;
        public static Point operator *(int k, Point a) => new(a.X * k, a.Y * k);
        public static Point operator -(Point p) => -1 * p;
        public static bool operator ==(Point a, Point b) => a.Equals(b);
        public static bool operator !=(Point a, Point b) => !a.Equals(b);
    }
}
