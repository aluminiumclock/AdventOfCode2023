string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
inputFile = "input.txt";
#endif

int result = 0;
string[] input = File.ReadAllLines(inputFile);

//Read Input into races
Int64 time = Int64.Parse(string.Join("", input[0].Split(new char[] { ':', ' ' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Skip(1)));
Int64 distance = Int64.Parse(string.Join("", input[1].Split(new char[] { ':', ' ' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Skip(1)));
Race race = new Race(time, distance);

result = race.NumberOfWaysToWin();

Console.WriteLine($"Result: {result}");
Console.ReadLine();


class Race
{
    public Int64 Time { get; set; }
    public Int64 Distance { get; set; }
    public Race(Int64 time, Int64 distance)
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
        quadAnswers.Add(QuadracticMinus(-1, Time, -1 * Distance));
        quadAnswers.Add(QuadracticPlus(-1, Time, -1 * Distance));

        //min solution is one above the quadratic formulas min solution
        int minSolution = (int)quadAnswers.Min() + 1;
        //max solution is int below the quadratic formula max solution (ignore the current int if it's has no decimals)
        int maxSolution = (int)Math.Ceiling(quadAnswers.Max()) - 1;

        int result = maxSolution - minSolution + 1; ;
        return result;
    }

    private double QuadracticMinus(Int64 a, Int64 b, Int64 c)
    {
        return ((-1 * b) - Math.Sqrt((b * b) - (4 * a * c))) / (2 * a);
    }

    private double QuadracticPlus(Int64 a, Int64 b, Int64 c)
    {
        return ((-1 * b) + Math.Sqrt((b * b) - (4 * a * c))) / (2 * a);
    }
}
