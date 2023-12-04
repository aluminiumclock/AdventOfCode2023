using System.Net.Http.Headers;

string inputFile;

#if DEBUG
inputFile = "text.txt";
#else
inputFile = "input.txt";
#endif

int result = 0;
string[] input = File.ReadAllLines(inputFile);
Dictionary<int, ScratchCard> ScratchCards = new();

/* add all scratch cards*/
for (int i = 0; i < input.Length; i++)
{
    ScratchCard card = new(input[i].Substring(input[i].IndexOf(':') + 1).Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries));
    ScratchCards.Add(i, card);
}


foreach (KeyValuePair<int,ScratchCard> card in ScratchCards)
{
    int MatchingNumbers = card.Value.MatchingNumbers();
    if( MatchingNumbers > 0 )
    {
        IEnumerable<int> cardsWon = Enumerable.Range(card.Key + 1, MatchingNumbers);
        foreach(int i in cardsWon)
        {
            ScratchCards[i].CardCount += card.Value.CardCount;
        }
    }
    result += card.Value.CardCount;
}




Console.WriteLine($"Result: {result}");
Console.ReadLine();


class ScratchCard
{
    private int[] _validNumbers;
    private int[] _chosenNumbers;
    public int CardCount;

    public ScratchCard(string[] cardText)
    {
        CardCount = 1;
        string[] vNumbers = cardText[0].Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        _validNumbers = Array.ConvertAll(vNumbers, s => int.Parse(s));
        string[] cNumbers = cardText[1].Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        _chosenNumbers = Array.ConvertAll(cNumbers, s => int.Parse(s));
    }

    public void PrintVaildNumbers()
    {
        Console.WriteLine("Valid Numbers:");
        foreach (int i in _validNumbers)
        {
            Console.WriteLine(i);
        }
    }

    public int MatchingNumbers()
    {
        return _chosenNumbers
            .Where(c => _validNumbers.Contains(c))
            .Count();
    }
}