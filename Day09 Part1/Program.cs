string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
    inputFile = "input.txt";
#endif

int result = 0;
string[] input = File.ReadAllLines(inputFile);
List<MapValue> map = new List<MapValue>();

foreach (string inputLine in input)
{
    List<int> history = inputLine.Split(" ").Select(int.Parse).ToList();
    map.Add(new MapValue(history));
}


result = map.Sum(x => x.NextValue());

Console.WriteLine(result);
Console.ReadLine();


class MapValue
{
    public List<int> History;

    public MapValue(List<int> history)
    {
        History = history;
    }

    public int NextValue()
    {
        var levels = new List<List<int>>();

        var differences = History.Zip(History.Skip(1), (x, y) => y - x).ToList();

        while (differences.Where(x => x != 0).Any())
        {
            levels.Add(differences);
            differences = differences.Zip(differences.Skip(1), (x, y) => y - x).ToList();
        }

        levels.Reverse();
        int prevValue = 0;
        foreach (var level in levels)
        {
            prevValue += level[^1];
        }
        int total = prevValue +  History.Last();

        return total;
    }
}