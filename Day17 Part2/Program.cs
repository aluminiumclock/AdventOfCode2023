
using System.Diagnostics.CodeAnalysis;


class Program
{
    const int MIN_JUMP = 4;
    const int MAX_JUMP = 10;

    private static void Main(string[] args)
    {
        string inputFile;
#if DEBUG
             inputFile = "test.txt";
#else
        inputFile = "input.txt";
#endif

        string input = File.ReadAllText(inputFile);
        Console.WriteLine(AdaptedDijksta(input, Coordinates.Zero, Coordinates.Zero));
    }

    public static int AdaptedDijksta(string input, Coordinates startingNode, Coordinates destination)
    {
        var map = ParseMap(input);
        var nodeHeatMap = map.ToDictionary(x => x.Key, x => int.MaxValue);
        destination = destination == Coordinates.Zero ? new Coordinates(map.Keys.Max(x => x.X), map.Keys.Max(x => x.Y)) : destination;
        //I was using a normal queue with an order by but this is clearly better
        var queue = new PriorityQueue<MapPoint, int>();

        // initial direction: right or down
        queue.Enqueue(new MapPoint(position: startingNode, direction: Coordinates.Right, directionCount: 0), 0);
        queue.Enqueue(new MapPoint(position: startingNode, direction: Coordinates.Down, directionCount: 0), 0);

        var visitedNodes = new HashSet<(Coordinates position, Coordinates direction, int directionCount)>();
        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            if (node.Position == destination && node.DirectionCount >= MIN_JUMP)
            {
                return node.HeatLoss;
            }
            foreach (var next in GetNextNodes(node))
            {
                if (visitedNodes.Contains((next.Position, next.Direction, next.DirectionCount))) { continue; }
                if (!map.ContainsKey(next.Position)) { continue; }

                int tempHeatLoss = node.HeatLoss + map[next.Position].HeatLoss;
                next.HeatLoss = tempHeatLoss;
                if (tempHeatLoss < nodeHeatMap[next.Position])
                {
                    nodeHeatMap[next.Position] = tempHeatLoss;
                }

                visitedNodes.Add((next.Position, next.Direction, next.DirectionCount));
                queue.Enqueue(next, tempHeatLoss);
            }
        }
        return int.MaxValue;
    }

    // returns possible next states based on the rules
    public static IEnumerable<MapPoint> GetNextNodes(MapPoint crucible)
    {
        List<MapPoint> nextNodes = new();

        if (crucible.DirectionCount >= MIN_JUMP)
        {
            foreach (var turn in Turns(crucible.Direction))
            {
                nextNodes.Add(new MapPoint(crucible.Position + turn, turn, 1));
            }
        }

        if (crucible.DirectionCount < MAX_JUMP)
        {
            nextNodes.Add(new MapPoint
            (
                crucible.Position + crucible.Direction,
                crucible.Direction,
                crucible.DirectionCount + 1
            )); ;
        }

        return nextNodes;
    }

    static Dictionary<Coordinates, CoordinateData> ParseMap(string input)
    {
        input = input.Replace("\r\n", "\n");
        var rows = input.Split('\n');
        int width = rows[0].Length;
        int height = rows.Length;

        var query =
            from row in Enumerable.Range(0, height)
            from column in Enumerable.Range(0, width)
            let position = new Coordinates(column, row)
            let cell = new CoordinateData { HeatLoss = rows[column][row] - '0' }
            select new KeyValuePair<Coordinates, CoordinateData>(position, cell);

        return query.ToDictionary(x => x.Key, x => x.Value);
    }

    static List<Coordinates> Turns(Coordinates direction)
    {
        if (direction == Coordinates.Up) { return new List<Coordinates> { Coordinates.Left, Coordinates.Right }; }
        else if (direction == Coordinates.Down) { return new List<Coordinates> { Coordinates.Left, Coordinates.Right }; }
        else if (direction == Coordinates.Left) { return new List<Coordinates> { Coordinates.Up, Coordinates.Down }; }
        else if (direction == Coordinates.Right) { return new List<Coordinates> { Coordinates.Up, Coordinates.Down }; }
        return new List<Coordinates>();
    }
}

record MapPoint
{
    public MapPoint(Coordinates position, Coordinates direction, int directionCount)
    {
        Position = position;
        Direction = direction;
        DirectionCount = directionCount;
    }

    public Coordinates Position { get; set; }
    public Coordinates Direction { get; set; }
    public int DirectionCount { get; set; }
    public int HeatLoss { get; set; } = 0;
}
class Coordinates
{
    public static readonly Coordinates Up = new Coordinates(-1, 0);
    public static readonly Coordinates Down = new Coordinates(1, 0);
    public static readonly Coordinates Left = new Coordinates(0, -1);
    public static readonly Coordinates Right = new Coordinates(0, 1);
    public static readonly Coordinates Zero = new Coordinates(0, 0);

    public Coordinates(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; set; }
    public int Y { get; set; }

    public static Coordinates operator +(Coordinates left, Coordinates right) =>
        new Coordinates(left.X + right.X, left.Y + right.Y);

    public override string ToString()
    {
        return $"({X},{Y})";
    }

    public static bool operator ==(Coordinates left, Coordinates right)
        => left.X == right.X && left.Y == right.Y;

    public static bool operator !=(Coordinates left, Coordinates right)
        => left.X != right.X || left.Y != right.Y;

    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Coordinates && Equals((Coordinates)obj);

    public bool Equals(Coordinates other) => this == other;

    public override int GetHashCode() => HashCode.Combine(X, Y);
}

class CoordinateData
{
    public int HeatLoss { get; set; }

}
