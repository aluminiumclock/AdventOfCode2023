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
        bool add = false;
        int start = j;
        int fullNumber = 0;
        if (Char.IsDigit(lines[i][j]))
        {
            while (j + 1 < lines[i].Length && Char.IsDigit(lines[i][j + 1]))
            {
                j++;
            }
            fullNumber = int.Parse(lines[i].Substring(start, j - start + 1));

            if (i > 0)
            {
                if (CheckLineForSymbol(lines[i - 1], start, j)) { add = true; }
            }

            if (CheckLineForSymbol(lines[i], start, j)) { add = true; }

            if (i + 1 < lines.Length)
            {
                if (CheckLineForSymbol(lines[i + 1], start, j)) { add = true; }
            }
        }

        if (add)
        {
            result += fullNumber;

        }
    }
}

Console.WriteLine(result);
Console.ReadLine();

bool CheckLineForSymbol(string v, int start, int end)
{
    start = start != 0 ? start - 1 : start;
    end = end + 1 == v.Length ? end : end + 1;
    for (int i = start; i <= end; i++)
    {
        if (IsSymbol(v[i])) { return true; }
    }
    return false;
}

bool IsSymbol(char symbol)
{
    return !(Char.IsLetterOrDigit(symbol) || symbol == '.');
}
