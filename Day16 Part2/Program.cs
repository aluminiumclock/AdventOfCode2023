using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Text;

string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
inputFile = "input.txt";
#endif
string input = File.ReadAllText(inputFile);

Coordinates Up = new Coordinates(-1, 0);
Coordinates Down = new Coordinates(1, 0);
Coordinates Left = new Coordinates(0, -1);
Coordinates Right = new Coordinates(0, 1);
Coordinates Zero = new Coordinates(0, 0);
int width = 0;
int height = 0;



Dictionary<Coordinates, CoordinateData> map = ParseMap(input);
List<(Coordinates Position, Coordinates Direction)> Beams = new List<(Coordinates, Coordinates)>();


int result = 0;
//West Side
for(int i = 0; i < height; i++)
{
    map.Values.ToList().ForEach(x => x.Reset());
    result = Math.Max(SolveBeam(new Coordinates(i, -1), Right), result);
}

//East Side
for (int i = 0; i < height; i++)
{
    map.Values.ToList().ForEach(x => x.Reset());
    result = Math.Max(SolveBeam(new Coordinates(i, height), Left), result);
}

//North Side
for (int i = 0; i < width; i++)
{
    map.Values.ToList().ForEach(x => x.Reset());
    result = Math.Max(SolveBeam(new Coordinates(-1, i), Down), result);
}

//South Side
for (int i = 0; i < width; i++)
{
    map.Values.ToList().ForEach(x => x.Reset());
    result = Math.Max(SolveBeam(new Coordinates(height, i), Up), result);
}


Console.WriteLine(result);


int SolveBeam(Coordinates Start, Coordinates Direction)
{
    //foreach(KeyValuePair<Coordinates,CoordinateData> mapv in map)
    //{
    //    mapv.Value.Reset();
    //}

    Beams.Add((Start, Direction));

    while (Beams.Count() > 0)
    {
        var beam = Beams.First();
        Beams.Remove(beam);
        PathBeam(beam.Position, beam.Direction);
    }
    
    Beams.Clear();
    return map.Where(x => x.Value.Intensity > 0).Count();
}


void PathBeam(Coordinates position, Coordinates beamDirection)
{
    //Create Local Versions
    Coordinates currentPosition = position + Zero;
    Coordinates direction = beamDirection + Zero;

    for (int count = 0; ; count++)
    {
        if (count > 10000) { Console.WriteLine("In a loop"); return; }
        Coordinates nextPosition = direction + currentPosition;

        if (!map.TryGetValue(nextPosition, out CoordinateData coordData)) { return; }
        if (coordData.Intensity > 200) { return; }

        coordData.Intensity++;
        char nextCharacter = coordData.C;

        if (nextCharacter == '.')
        {
            currentPosition = nextPosition;
            continue;
        }
        if (nextCharacter == '\\')
        {
            currentPosition = nextPosition;
            //Can't use switch as these can't be static in project without top-level statements
            if (direction == Down) { direction = Right; continue; }
            if (direction == Up) { direction = Left; continue; }
            if (direction == Right) { direction = Down; continue; }
            if (direction == Left) { direction = Up; continue; }
            continue;
        }
        if (nextCharacter == '/')
        {
            currentPosition = nextPosition;
            if (direction == Down) { direction = Left; continue; }
            if (direction == Up) { direction = Right; continue; }
            if (direction == Right) { direction = Up; continue; }
            if (direction == Left) { direction = Down; continue; }
            continue;
        }
        if (nextCharacter == '|')
        {
            if (direction == Down || direction == Up)
            {
                currentPosition = nextPosition;
                continue;
            }

            if (coordData.Reflected) { return; }
            coordData.Reflected = true;
            Beams.Add((nextPosition, Up));
            Beams.Add((nextPosition, Down));
            return;
        }
        if (nextCharacter == '-')
        {
            if (direction == Left || direction == Right)
            {
                currentPosition = nextPosition;
                continue;
            }
            if (coordData.Reflected) { return; }
            coordData.Reflected = true;
            Beams.Add((nextPosition, Left));
            Beams.Add((nextPosition, Right));
            return;
        }
    }
}

Dictionary<Coordinates, CoordinateData> ParseMap(string input)
{
    input = input.Replace("\r\n", "\n");
    var rows = input.Split('\n');
    width = rows[0].Length;
    height = rows.Length;

    var query =
        from row in Enumerable.Range(0, height)
        from column in Enumerable.Range(0, width)
        let position = new Coordinates(column, row)
        let cell = new CoordinateData { C = rows[column][row], Intensity = 0 }
        select new KeyValuePair<Coordinates, CoordinateData>(position, cell);

    return query.ToDictionary(x => x.Key, x => x.Value);
}


void PrintMapChacters()
{
    StringBuilder stringBuilder = new StringBuilder();
    for (int i = 0; i < height; i++)
    {
        for (int j = 0; j < width; j++)
        {
            stringBuilder.Append(map[new Coordinates(i, j)].C);
        }
        stringBuilder.AppendLine();
    }
    Console.WriteLine(stringBuilder.ToString());
}
void PrintMapEnergized()
{
    StringBuilder stringBuilder = new StringBuilder();
    for (int i = 0; i < height; i++)
    {
        for (int j = 0; j < width; j++)
        {
            stringBuilder.Append(map[new Coordinates(i, j)].Intensity > 0 ? '#' : '.');
        }
        stringBuilder.AppendLine();
    }
    Console.WriteLine(stringBuilder.ToString());
}

class Coordinates
{
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
    public char C { get; set; }
    public int Intensity { get; set; }
    public bool Reflected { get; set; } = false;

    public void Reset()
    {
        Intensity = 0;
        Reflected = false;
    }
}
