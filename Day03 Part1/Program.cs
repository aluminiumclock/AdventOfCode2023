string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
    inputFile = "input.txt";
#endif

int result = 0;

string[] lines = File.ReadAllLines(inputFile);
for (int i = 0; i < lines.Length; i++)
{
    for (int j = 0; j < lines[i].Length; j++)
    {
        if (Char.IsDigit(lines[i][j]))
        {
            //keep going right until you reach the end of the number or the end of the line
            int start = j;
            while (j + 1 < lines[i].Length && Char.IsDigit(lines[i][j + 1]))
            {
                j++;
            }
            int fullNumber = int.Parse(lines[i].Substring(start, j - start + 1));

            //Check lines around for a nearby gear
            if (i > 0)
            {
                if (CheckLineForSymbol(lines[i - 1], start, j))
                {
                    result += fullNumber;
                    continue;
                }
            }

            if (CheckLineForSymbol(lines[i], start, j))
            {
                result += fullNumber;
                continue;
            }

            if (i + 1 < lines.Length)
            {
                if (CheckLineForSymbol(lines[i + 1], start, j))
                {
                    result += fullNumber;
                    continue;
                }
            }
        }
    }
}

Console.WriteLine(result);
Console.ReadLine();

bool CheckLineForSymbol(string v, int start, int end)
{
    //boundry checks
    start = start != 0 ? start - 1 : start;
    end = end + 1 == v.Length ? end : end + 1;

    //check for a symbol in with the range of the string
    return v.Substring(start, end - start + 1).Where(x => IsSymbol(x)).Any();
}

bool IsSymbol(char symbol)
{
    return !(Char.IsLetterOrDigit(symbol) || symbol == '.');
}
