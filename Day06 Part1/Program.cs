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

        List<double> quadAnswers = new();
        quadAnswers.Add(QuadracticMinus(1, -1 * Time, Distance));
        quadAnswers.Add(QuadracticPlus(1, -1 * Time, Distance));

        //min solution is one above the quadratic formulas min solution
        int minSolution = (int)quadAnswers.Min() + 1;
        //max solution is int below the quadratic formula max solution (ignore the current int if it's has no decimals)
        int maxSolution = (int)Math.Ceiling(quadAnswers.Max()) - 1;

        int result = maxSolution - minSolution + 1; ;
        return result;
    }

    private double QuadracticMinus(int a, int b, int c)
    {
        return ((-1 * b) - Math.Sqrt((b * b) - (4 * a * c))) / (2 * a);
    }

    private double QuadracticPlus(int a, int b, int c)
    {
        return ((-1 * b) + Math.Sqrt((b * b) - (4 * a * c))) / (2 * a);
    }
}
