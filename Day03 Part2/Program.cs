string inputFile;


    inputFile = "input.txt";


int result = 0;

string[] lines = File.ReadAllLines(inputFile);
Dictionary<int, List<NumberInString>> numbersInText = new();

LoadNumbers(lines, numbersInText);

for (int i = 0; i < lines.Length; i++)
{
    for (int j = 0; j < lines[i].Length; j++)
    {
        if (lines[i][j] == '*')
        {
            int numbersByGear = 0;
            int fullNumber = 1;

            if (i > 0)
            {
                StringNumberData lt = NearbyNumbers(i - 1, j);
                fullNumber *= lt.Fullnumber;
                numbersByGear += lt.numbersAdjGearOnLine;
            }

            StringNumberData clt = NearbyNumbers(i, j);
            fullNumber *= clt.Fullnumber;
            numbersByGear += clt.numbersAdjGearOnLine;

            if (i + 1 < lines.Length)
            {

                StringNumberData lt = NearbyNumbers(i + 1, j);
                fullNumber *= lt.Fullnumber;
                numbersByGear += lt.numbersAdjGearOnLine;
            }

            if (numbersByGear > 1)
            {
                result += fullNumber;

            }
        }
    }
}

Console.WriteLine(result);
Console.ReadLine();

StringNumberData NearbyNumbers (int line, int position)
{
    List<NumberInString> numbers = numbersInText[line];

    return new StringNumberData
    {
        total = numbers.Where(n => n.Touching(position)).Aggregate<NumberInString, int>(1, (p, n) => p * n.Number),
        numbersAdjGearOnLine = numbers.Where(n => n.Touching(position)).Count()
    };
}

static void LoadNumbers(string[] lines, Dictionary<int, List<NumberInString>> numbersInText)
{
    for (int x = 0; x < lines.Length; x++)
    {
        string v = lines[x];
        List<NumberInString> numbers = new();
        for (int i = 0; i < v.Length; i++)
        {
            if (Char.IsDigit(v[i]))
            {
                int start = i;
                while (i + 1 < v.Length && Char.IsDigit(v[i + 1]))
                {
                    i++;
                }
                NumberInString numberInString = new NumberInString
                {
                    Number = Int32.Parse(v.Substring(start, i - start + 1)),
                    Start = start,
                    End = i
                };
                numbers.Add(numberInString);
            }
        }
        numbersInText.Add(x, numbers);
    }
}

struct NumberInString
{
    public int Number;
    public int Start;
    public int End;

    public bool Touching(int position)
    {
        return Start - 1 <= position
                && position <= End + 1;
    }

    public override string ToString()
    {
        return $"Number {Number} starts at {Start} and ends at {End}";
    }
}

struct StringNumberData
{
    public int total;
    public int numbersAdjGearOnLine;
    public int Fullnumber => total == 0 ? 1 : total;
}