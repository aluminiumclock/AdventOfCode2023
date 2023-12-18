using System.Diagnostics.CodeAnalysis;
using System.Numerics;

string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
inputFile = "input.txt";
#endif

string[] input = File.ReadAllLines(inputFile);
var instructions = input.Select(x => x.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                        .Select(y => (Direction: y[0], Distance: long.Parse(y[1]), Color: y[2][1..^1]));

var updatedInstructions = instructions.Select(x => (GetDirection(x.Color[^1]), GetDistance(x.Color), x.Color));

var polygon = new List<Vertex>();
var start = Coordinates.Zero;
foreach (var (Direction, Distance, Color) in updatedInstructions)
{
    var nextPoint = NextPoint(start, Distance, Direction);
    polygon.Add(
        new Vertex
        {
            Start = start,
            End = nextPoint,
            Length = Distance,
            Direction = Direction,
            Color = Color
        }
    );
    start = nextPoint;
}

string GetDirection(char color) => color switch
{
    '0' => "R",
    '1' => "D",
    '2' => "L",
    '3' => "U"
};

long GetDistance(string color) =>
    long.Parse(color[1..^1], System.Globalization.NumberStyles.HexNumber);


long area = Shoelace(polygon.Select(x => x.Start).ToList());
long boundaryPoints = polygon.Sum(x => x.Length);
//re-arrange picks formula
long result = area + boundaryPoints / 2 + 1;

Console.WriteLine(result);
Console.ReadLine();


//geeks for geeks alogrithim
long Shoelace(List<Coordinates> vertices)
{
    // Initialize area
    double area = 0.0;
    long[] X = vertices.Select(x => x.X).ToArray();
    long[] Y = vertices.Select(x => x.Y).ToArray();
    if (Y.Length != X.Length) { throw new Exception(); }
    long n = X.Length;


    // Calculate value of shoelace formula
    long j = n - 1;

    for (long i = 0; i < n; i++)
    {
        area += (X[j] + X[i]) * (Y[j] - Y[i]);

        // j is previous vertex to i
        j = i;
    }

    // Return absolute value
    return (long)Math.Abs(area / 2.0);
}

Coordinates NextPoint(Coordinates start, long distance, string direction)
{
    return start + (ConvetToCoordinateDirection(direction) * distance);
}

Coordinates ConvetToCoordinateDirection(string direction) =>
direction switch
{
    "R" => Coordinates.Right,
    "L" => Coordinates.Left,
    "U" => Coordinates.Up,
    "D" => Coordinates.Down
};


record Vertex
{
    public required Coordinates Start { get; set; }
    public required Coordinates End { get; set; }
    public required long Length { get; set; }
    public required string Direction { get; set; }
    public required string Color { get; set; }

}


class Coordinates
{
    public static readonly Coordinates Up = new(-1, 0);
    public static readonly Coordinates Down = new(1, 0);
    public static readonly Coordinates Left = new(0, -1);
    public static readonly Coordinates Right = new(0, 1);
    public static readonly Coordinates Zero = new(0, 0);

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
