using System.Net.Http.Headers;

string inputFile;

#if DEBUG
inputFile = "text.txt";
#else
inputFile = "input.txt";
#endif

int result = 0;
string[] input = File.ReadAllLines(inputFile);


foreach (string line in input)
{
    ScratchCard card = new(line.Substring(line.IndexOf(':') + 1).Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
    result += card.PointsTotal();
    
}


Console.WriteLine($"Result: {result}");
Console.ReadLine();


class ScratchCard
{
    private int[] _validNumbers;
    private int[] _chosenNumbers;

    public ScratchCard(string[] cardText)
    {
        string[] vNumbers = cardText[0].Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        _validNumbers = Array.ConvertAll(vNumbers, s => int.Parse(s));
        string[] cNumbers = cardText[1].Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        _chosenNumbers = Array.ConvertAll(cNumbers, s => int.Parse(s));
    }

    public void PrintVaildNumbers()
    {
        Console.WriteLine("Valid Numbers:");
        foreach(int i in _validNumbers)
        {
            Console.WriteLine(i);
        }
    }

    public int PointsTotal()
    {
        return (int)Math.Pow(2, MatchingNumbers() - 1);
    }

    private int MatchingNumbers()
    {
        return _chosenNumbers
            .Where(c => _validNumbers.Contains(c))
            .Count();
    }
}