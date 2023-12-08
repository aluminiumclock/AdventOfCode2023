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
List<int> cycles = new List<int>();


foreach(var ANode in ANodes)
{
    int steps = 0;
    string nextNode = ANode;

    do
    {
        char instruction = instructions[steps % instructions.Length];
        nextNode = instruction == 'R' ? nodes[nextNode].Right : nodes[nextNode].Left;
        steps++;
    }
    while (!(nextNode[2] == 'Z'));
    
    cycles.Add(steps);
}


long result = LowestCommonMultiple(cycles);

Console.WriteLine(result);
Console.ReadLine();


long LowestCommonMultiple(List<int> cycles)
{
    return cycles.Select(x => (long)x).Aggregate((x, y) => ((x * y) / GreatestCommonDivisor(x, y)));
}

//credit for this method goes to https://stackoverflow.com/questions/18541832/c-sharp-find-the-greatest-common-divisor
long GreatestCommonDivisor(long x, long y)
{
    while (x != 0 && y != 0)
    {
        if (x > y)
            x %= y;
        else
            y %= x;
    }

    return x | y;
}

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