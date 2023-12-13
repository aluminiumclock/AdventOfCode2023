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
    IEnumerable<int> possibleMirrorRowIndexs = puzzle.Rows.Select((x, i) => (x, index: i)).Where((x, i) => i + 1 < puzzle.Rows.Count && x.x == puzzle.Rows[i + 1]).Select(y => y.index);
    foreach (var rowIndex in possibleMirrorRowIndexs)
    {
        int i = 1;
        bool isMirror = true;

        while (isMirror && 0 <= rowIndex - i && rowIndex + 1 + i < puzzle.Rows.Count)
        {
            if (puzzle.Rows[rowIndex - i] != puzzle.Rows[rowIndex + 1 + i])
            {
                isMirror = false;
            }
            i++;
        }
        if (isMirror)
        {
            result += (rowIndex + 1) * ROW_VALUE;
        }
    }

    //Columns
    IEnumerable<int> possibleMirrorcolumnIndexs = puzzle.Columns.Select((x, i) => (x, index: i)).Where((x, i) => i + 1 < puzzle.Columns.Count && x.x == puzzle.Columns[i + 1]).Select(y => y.index);
    foreach (var columnIndex in possibleMirrorcolumnIndexs)
    {
        int i = 1;
        bool isMirror = true;

        while (isMirror && 0 <= columnIndex - i && columnIndex + 1 + i < puzzle.Columns.Count)
        {
            if (puzzle.Columns[columnIndex - i] != puzzle.Columns[columnIndex + 1 + i])
            {
                isMirror = false;
            }
            i++;
        }
        if (isMirror)
        {
            result += (columnIndex + 1) * COLUMN_VALUE;
        }
    }
}

Console.WriteLine(result);
Console.ReadKey();

class Puzzle
{
    public List<string> Rows = new();
    public List<string> Columns = new();

    public Puzzle(List<string> rows)
    {
        Rows = rows;

        for(int i = 0; i < Rows[0].Length; i++)
        {
            string column = String.Join("",Rows.Select(x => x[i]).ToArray<char>());
            Columns.Add(column);
        }
    }
}