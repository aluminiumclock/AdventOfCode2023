using System.Runtime.CompilerServices;
using System.Text;

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
    var lineSplit = line.Split(new char[] { '=',','}, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    Node newNode = new Node(lineSplit[1][1..], lineSplit[2][..3]);
    nodes.Add(lineSplit[0][..3], newNode);
}

string nextNode = "AAA";
int steps = 0;

while (nextNode != "ZZZ")
{
    char instruction = instructions[steps % instructions.Length];
    nextNode = instruction == 'R' ? nodes[nextNode].Right : nodes[nextNode].Left;
    steps++;
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
}