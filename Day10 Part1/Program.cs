using System.Collections.Generic;
using System.Drawing;
using System.Runtime.ExceptionServices;

string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
    inputFile = "input.txt";
#endif

string[] lines = File.ReadAllLines(inputFile);
Dictionary<(int, int), Pipe> pipeGrid = new();
(int x, int y) start = (-1, -1);

for(int i = 0; i < lines.Length; i++)
{
    for (int j = 0; j < lines[i].Length ; j++)
    {
        if (lines[i][j] == 'S')
        {
            start = (lines.Length - i - 1, j);
        }
        pipeGrid.Add((lines.Length - i - 1, j), new Pipe(lines[i][j]));
    }
}


List<string> CardinalDiections = new()
{
    ("North"),
    ("West"),
    ("East"),
    ("South")
};
Dictionary<string,(int x, int y)> Directions = new()
{
    {"North", (1, 0) },
    {"West", (0, -1)},
    {"East", (0, 1) },
    {"South", (-1, 0) },
    {"", (0, 0) }
};
//var startPipe = pipeGrid[start];
//startPipe.Distance = 0;


foreach (var direction in CardinalDiections)
{
    (int x, int y) next = (Directions[direction].x + start.x, Directions[direction].y + start.y);
    if(!pipeGrid.ContainsKey(next))
    {
        continue;
    }
    if (!pipeGrid[next].ValidFromDirection(direction))
    {
        continue;
    }
    Pipe nextPipe = pipeGrid[start];
    string nextDirection = direction;
    int distance = 0;

    while (nextPipe.Distance < 0)
    {
        nextPipe.Distance = distance;
        if (pipeGrid.TryGetValue(next, out Pipe possibleNextPipe))
        {
            nextPipe = possibleNextPipe;
            nextDirection = nextPipe.ToDirection(nextDirection);
            next = (next.x + Directions[nextDirection].x, next.y + Directions[nextDirection].y);
            distance++;
        }
    }
}



int result = (pipeGrid.Max(x => x.Value.Distance) + 1) / 2;
var resultgrid = pipeGrid.Where(x => x.Value.Distance >= 0);

Console.WriteLine(result);
Console.ReadLine();



class Pipe
{
    public Pipe(char type)
    {
        PipeType = type;
    }

    public char PipeType { get; set; }
    public int Distance { get; set; } = -1;

    public string ToDirection(string direction)
    {
       if (PipeType == '|' && direction == "North"){ return "North"; }
       if (PipeType == '|' && direction == "South") { return "South"; }
       if (PipeType == '-' && direction == "West") { return "West"; }
       if (PipeType == '-' && direction == "East") { return "East"; }
       if (PipeType == 'L' && direction == "South") { return "East"; }
       if (PipeType == 'L' && direction == "West") { return "North"; }
       if (PipeType == 'J' && direction == "South") { return "West"; }
       if (PipeType == 'J' && direction == "East") { return "North"; }
       if (PipeType == '7' && direction == "North") { return "West"; }
       if (PipeType == '7' && direction == "East") { return "South"; }
       if (PipeType == 'F' && direction == "West") { return "South"; }
       if (PipeType == 'F' && direction == "North") { return "East"; }       
       return "";
    }

    public bool ValidFromDirection(string direction)
    {
       if (PipeType == '|' && direction == "North") { return true; }
       if (PipeType == '|' && direction == "South") { return true; }
       if (PipeType == '-' && direction == "West")  { return true; }
       if (PipeType == '-' && direction == "East")  { return true; }
       if (PipeType == 'L' && direction == "South") { return true; }
       if (PipeType == 'L' && direction == "West")  { return true; }
       if (PipeType == 'J' && direction == "South") { return true; }
       if (PipeType == 'J' && direction == "East")  { return true; }
       if (PipeType == '7' && direction == "North") { return true; }
       if (PipeType == '7' && direction == "East")  { return true; }
       if (PipeType == 'F' && direction == "West")  { return true; }
       if (PipeType == 'F' && direction == "North") { return true; }      
       return false;
    }
}
