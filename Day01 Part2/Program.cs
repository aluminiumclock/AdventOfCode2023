string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
    inputFile = "input.txt";
#endif

int result = 0;
string[] input = File.ReadAllLines(inputFile);

Dictionary<string, int> stringDigits = new()
            {
                {"one", 1 },
                {"two", 2 },
                {"three", 3 },
                {"four", 4 },
                {"five", 5 },
                {"six", 6 },
                {"seven", 7 },
                {"eight", 8 },
                {"nine", 9 },
            };

for (int i = 1; i <= 9; i++)
{
    stringDigits.Add(i.ToString(), i);
}

foreach (string line in input)
{
    int firstNumber = 0;
    int firstIndex = Int32.MaxValue;
    int lastNumber = 0;
    int lastIndex = Int32.MinValue;

    foreach (var digit in stringDigits)
    {
        int index = line.IndexOf(digit.Key);

        if (index == -1)
        {
            continue;
        }

        if (index < firstIndex)
        {
            firstIndex = index;
            firstNumber = digit.Value;
        }

        index = line.LastIndexOf(digit.Key);

        if (index > lastIndex)
        {
            lastIndex = index;
            lastNumber = digit.Value;
        }
    }
    result += firstNumber * 10 + lastNumber;
}

Console.WriteLine(result);
Console.ReadLine();
