using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;

string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
inputFile = "input.txt";
#endif

string[] input = File.ReadAllLines(inputFile);
var instructions = input[0];
Dictionary<string, Node> nodes = new();

foreach (string line in input.Skip(2))
{
    var lineSplit = line.Split(new char[] { '=', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    Node newNode = new Node(lineSplit[1][1..], lineSplit[2][..3]);
    nodes.Add(lineSplit[0][..3], newNode);
}



var ANodes = nodes.Where(x => x.Key[2] == 'A').Select(x => x.Key);
bool AllZNodes = false;
int steps = 0;

while (!AllZNodes)
{
    char instruction = instructions[steps % instructions.Length];
    ANodes = ANodes.Select(x => nodes[x].Next(instruction));
    AllZNodes = !ANodes.Where(x => x[2] != 'Z').Any();
    steps++;
    if(steps % 10000 == 0)
    {
        Console.WriteLine(steps);
    }
}



Console.WriteLine(steps);
Console.ReadLine();

class Node
{
    public Node(string left, string right)
    {
        Left = left;
        Right = right;
    }

    public string Left { get; set; }
    public string Right { get; set; }

    public string Next(char instruction)
    {
        return instruction == 'R' ? Right : Left;
    }
}