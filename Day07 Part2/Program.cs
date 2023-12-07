using System.ComponentModel;
using System.Runtime.InteropServices;

string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
inputFile = "input.txt";
#endif

int result = 0;
string[] input = File.ReadAllLines(inputFile);
List<Hand> hands = new();

foreach (string line in input)
{
    var handData = line.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    hands.Add(new Hand(handData[0], int.Parse(handData[1])));
}

hands = hands.OrderByDescending(x => x.handType).ThenBy(x => x.CardRank).ToList();
for (int i = 0; i < hands.Count; i++)
{
    result += hands[i].bid * (i + 1);
}

Console.WriteLine(result);
Console.ReadLine();



class Hand
{
    public Dictionary<int, int> cards = new();
    public int bid;
    public Int64 CardRank => HandRank();

    public HandType handType { get; private set; }

    public Hand(string cardsText, int bid)
    {
        this.bid = bid;
        for (int i = 0; i < cardsText.Length; i++)
        {
            cards.Add(i, CardValue(cardsText[i]));
        }
        SetHandType();
    }

    private void SetHandType()
    {

        IEnumerable<IGrouping<int, int>> grouped;

        //replace jokes with the most common card in the hand
        int jokers = cards.Where(x => x.Value == 1).Count();
        if (jokers == 5)
        {
            handType = HandType.FiveOfAKind;
            return;
        }
        else if (jokers > 0)
        {
            int mostCommonCard = cards.Select(x => x.Value).GroupBy(x => x).Select(grp => new { Card = grp.Key, Count = grp.Count() }).Where(x => x.Card != 1).OrderBy(x => x.Count).Last().Card;
            grouped = cards.Select(x => x.Value == 1 ? mostCommonCard : x.Value).GroupBy(x => x);
        }
        else
        {
            grouped = cards.Select(x => x.Value).GroupBy(x => x);
        }


        if (grouped.Where(x => x.Count() == 5).Any())
        {
            handType = HandType.FiveOfAKind;
            return;
        }

        if (grouped.Where(x => x.Count() == 4).Any())
        {
            handType = HandType.FourOfAKind;
            return;
        }

        if (grouped.Where(x => x.Count() == 3).Any() && grouped.Where(x => x.Count() == 2).Any())
        {
            handType = HandType.FullHouse;
            return;
        }

        if (grouped.Where(x => x.Count() == 3).Any())
        {
            handType = HandType.ThreeOfAKind;
            return;
        }

        if (grouped.Where(x => x.Count() == 2).Count() == 2)
        {
            handType = HandType.TwoPair;
            return;
        }

        if (grouped.Where(x => x.Count() == 2).Any())
        {
            handType = HandType.OnePair;
            return;
        }

        handType = HandType.HighCard;
    }

    private Int64 HandRank()
    {
        Int64 total = 0;

        for (int i = 0; i < cards.Count; i++)
        {
            total += cards[i] * (int)Math.Pow(15, (cards.Count - i));
        }

        return total;
    }

    public int CardValue(char c)
    {
        if (Char.IsDigit(c)) { return int.Parse(c.ToString()); }

        Dictionary<char, int> cardValueMap = new Dictionary<char, int>
        {
            {'A',14},
            {'K',13},
            {'Q',12},
            {'J',1},
            {'T',10},

        };

        return cardValueMap[c];
    }

}

enum HandType
{
    FiveOfAKind,
    FourOfAKind,
    FullHouse,
    ThreeOfAKind,
    TwoPair,
    OnePair,
    HighCard
}