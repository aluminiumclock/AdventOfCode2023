string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
    inputFile = "input.txt";
#endif

int result = 0;
string[] input = File.ReadAllLines(inputFile);

//Read Input into races
List<int> times = input[0].Split(new char[] { ':',' '},StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(x => int.Parse(x)).ToList();
List<int> distances = input[1].Split(new char[] { ':', ' ' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(x => int.Parse(x)).ToList();
List<Race> races = times.Zip(distances, ((x, y) => new Race(x, y))).ToList();

result = races.Select(x => x.NumberOfWaysToWin()).Aggregate((x, y) => x * y);

Console.WriteLine($"Result: {result}");
Console.ReadLine();


class Race
{
    public int Time { get; set; }
    public int Distance { get; set; }
    public Race(int time, int distance)
    {
        Time = time;
        Distance = distance;
    }

    public override string ToString()
    {
        return $"Race time: {Time}; Current distance record {Distance}";
    }

    public int NumberOfWaysToWin()
    {

        //set better mins and max here.
        int waysToWin = 0;
        for(int hold = 1; hold < Time; hold++)
        {
            if(((Time - hold) * hold) > Distance) { waysToWin++; }
        }
        return waysToWin;
    }
}
