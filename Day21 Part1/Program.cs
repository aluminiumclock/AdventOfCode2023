using AOC_Helper;

string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
    inputFile = "input.txt";
#endif

const int STEPS = 64;
Dictionary<Coordinates, CoordinateData> grid = new();
grid = ParseGrid(File.ReadAllLines(inputFile));

Coordinates Start = grid.Where(x => x.Value.Type == 'S')
                        .Select(x => x.Key)
                        .Single();



int result = PossiblePlots(grid, Start, STEPS);

Console.WriteLine(result);
Console.ReadLine();


int PossiblePlots(Dictionary<Coordinates, CoordinateData> grid, Coordinates start, int steps)
{
    List<Coordinates> plots = [start];
    for (int i = 0; i < steps; i++)
    {
        plots = GetNeighbours(plots.ToArray());
    }
    return plots.Count();
}

Dictionary<Coordinates, CoordinateData> ParseGrid(string[] lines)
{
    Dictionary<Coordinates, CoordinateData> localGrid = new();
    for (int i = 0; i < lines.Count(); i++ )
    {
        for(int j = 0; j < lines[i].Length; j++ )
        {
            localGrid.Add(new Coordinates(j, i), new CoordinateData() { Type = lines[i][j] }) ;
        }
    }
    MapNeighbours(localGrid);
    return localGrid;
}

void MapNeighbours(Dictionary<Coordinates, CoordinateData> grid)
{
    foreach (KeyValuePair<Coordinates,CoordinateData> x in grid)
    {
        x.Value.neighbours = GetNeighbours(x.Key);
    }
}

List<Coordinates> GetNeighbours(params Coordinates[] keys)
{
    List<Coordinates> neighbours = new();

    foreach (Coordinates coord in keys)
    {
        foreach (var direction in Coordinates.CardinalDirections)
        {
            var possibleNeighbour = coord + direction;
            if (grid.TryGetValue(possibleNeighbour, out CoordinateData neighbour))
            {
                if (neighbour.Type != '#' && !neighbours.Contains(possibleNeighbour))
                {
                   neighbours.Add(possibleNeighbour);
                }
            }
        }
    }
    return neighbours;
}
class CoordinateData
{
    public required char Type;
    public List<Coordinates> neighbours { get; set; } = new();
}