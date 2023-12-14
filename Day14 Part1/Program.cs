string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
    inputFile = "input.txt";
#endif

int result = 0;
string[] input = File.ReadAllLines(inputFile);
int width = input[0].Length;
int height = input.Length;
char[,] platform = new char[width, height];


for (int x = 0; x < width; x++)
{
    for (int y = 0; y < height; y++)
    {
        platform[x,y] = input[y][x];
    }
}

List<char[]> Columns = new();
for (int row = 0; row < height; row++)
{
    var column = Enumerable.Range(0, width)
                    .Select(x => platform[row, x])
                    .ToArray();
    Columns.Add(column);
}

List<char[]> Rows = new();
for (int column = 0; column < width; column++)
{
    var row = Enumerable.Range(0, height)
                    .Select(x => platform[x, column])
                    .ToArray();
    Rows.Add(row);
}

foreach(var column in Columns)
{
    List<int> squareBoulders = new List<int>() { -1 };
    squareBoulders.AddRange(column.Select((v, i) => (letter: v, index: i))
                                .Where(x => x.letter == '#')
                                .Select(x => x.index)
                                .ToList()
                            );
    squareBoulders.Add(int.MaxValue);

    var squareBoulderRange = squareBoulders.Zip(squareBoulders.Skip(1));

    List<int> roundBoulders = column.Select((v, i) => (v, i))
                            .Where(x => x.v == 'O')
                            .Select(x => x.i)
                            .ToList();
    
    foreach(var boulderRange in squareBoulderRange)
    {
        int roundBouldersInRange = roundBoulders.Where(x => x > boulderRange.First && x < boulderRange.Second).Count();
        int MaxValue = height - (boulderRange.First + 1);
        int Total = roundBouldersInRange * (MaxValue + (MaxValue - roundBouldersInRange + 1)) / 2;
        result += Total;
    }


}

Console.WriteLine(result);
Console.ReadLine();
