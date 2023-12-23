using System.Diagnostics.CodeAnalysis;

namespace AOC_Helper
{

    public class Coordinates
    {
        public static readonly Coordinates Up = new(-1, 0);
        public static readonly Coordinates Down = new(1, 0);
        public static readonly Coordinates Left = new(0, -1);
        public static readonly Coordinates Right = new(0, 1);
        public static readonly Coordinates Zero = new(0, 0);
        public static readonly List<Coordinates> CardinalDirections =
        [
            Up, Down, Left, Right
        ];

        public Coordinates(long x, long y)
        {
            X = x;
            Y = y;
        }

        public long X { get; set; }
        public long Y { get; set; }

        public static Coordinates operator +(Coordinates left, Coordinates right) =>
            new(left.X + right.X, left.Y + right.Y);

        public static Coordinates operator *(Coordinates coordinates, long number)
            => new(coordinates.X * number, coordinates.Y * number);

        public override string ToString()
        {
            return $"({X},{Y})";
        }

        public static bool operator ==(Coordinates left, Coordinates right)
            => left.X == right.X && left.Y == right.Y;

        public static bool operator !=(Coordinates left, Coordinates right)
            => left.X != right.X || left.Y != right.Y;

        public override bool Equals([NotNullWhen(true)] object? obj) => obj is Coordinates coordinates && Equals(coordinates);

        public bool Equals(Coordinates other) => this == other;

        public override int GetHashCode() => HashCode.Combine(X, Y);
    }

}
