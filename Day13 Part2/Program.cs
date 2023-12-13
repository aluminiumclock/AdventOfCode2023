using System.Data;

string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
inputFile = "input.txt";
#endif

const int ROW_VALUE = 100;
const int COLUMN_VALUE = 1;


string input = File.ReadAllText(inputFile);
List<Puzzle> puzzles = input.Split("\r\n\r\n").Select(x => new Puzzle(x.Split("\r\n").ToList())).ToList();
int result = 0;


foreach (var puzzle in puzzles)
{
    //Rows
    for (int rowIndex = 0; rowIndex < puzzle.Rows.Count; rowIndex++)
    {
        int i = 0;
        int diff = 0;
        while (diff <= 2 &&  0 <= rowIndex - i && rowIndex + 1 + i < puzzle.Rows.Count)
        {
            diff += HammingDistance(puzzle.Rows[rowIndex - i], puzzle.Rows[rowIndex + 1 + i]);
            i++;
        }
        if (diff == 1)
        {
            result += (rowIndex + 1) * ROW_VALUE;
        }
    }

    //Columns
    for(int columnIndex = 0; columnIndex < puzzle.Columns.Count; columnIndex++)
    {
        int i = 0;
        int diff = 0;

        while (diff <= 2 && 0 <= columnIndex - i && columnIndex + 1 + i < puzzle.Columns.Count)
        {
            diff += HammingDistance(puzzle.Columns[columnIndex - i], puzzle.Columns[columnIndex + 1 + i]);
            i++;
        }
        if (diff == 1)
        {
            result += (columnIndex + 1) * COLUMN_VALUE;
        }
    }
}

Console.WriteLine(result);
Console.ReadKey();

int HammingDistance(string a, string b)
{
    if(a.Length != b.Length)
    {
        throw new ArgumentException();
    }

    int difference = (a.Zip(b).Where(x => x.First != x.Second)).Count();
    return difference;
}

class Puzzle
{
    public List<string> Rows = new();
    public List<string> Columns = new();

    public Puzzle(List<string> rows)
    {
        Rows = rows;

        for (int i = 0; i < Rows[0].Length; i++)
        {
            string column = String.Join("", Rows.Select(x => x[i]).ToArray<char>());
            Columns.Add(column);
        }
    }
}