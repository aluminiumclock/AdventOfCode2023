
using System.Diagnostics;

string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
    inputFile = "input.txt";
#endif

string[] input = File.ReadAllText(inputFile).Replace("\r\n","\n").Split("\n\n");

Dictionary<string, Instructions> workflows = ParseWorkflows(input[0]);
List<Part> parts = ParseParts(input[1]);

//Every part starts in the "in" workflow
parts.ForEach(x => x.Actions.Enqueue(() => ProcessWorkflow(x,"in")));

foreach (Part part in parts)
{
    while (part.Actions.Count > 0)
    {
        var x = part.Actions.Dequeue();
        x.Invoke();
    }
}

int result = parts.Where(x => x.Accepted).Sum(x => x.Rating());

Console.WriteLine(result);
Console.ReadLine();

List<Part> ParseParts(string input)
{
    var lines = input.Split('\n');
    return lines.Select(x => new Part(x.Split(new char[] {',','{','}'}, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))).ToList();
}

Dictionary<string, Instructions> ParseWorkflows(string v)
{
    var lines = v.Split("\n");
    return lines.Select(x => ParseInstructions(x)).ToDictionary(y => y.Item1, y => y.Item2);
}

(string, Instructions) ParseInstructions(string workflow) {
    var data = workflow.Split(new char[] {'{','}',','}, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    return ((string)data[0], new Instructions(data[1..]));
}

void ProcessWorkflow(Part part, string worklowId)
{
    var instructions = workflows[worklowId].instructions;
    foreach (var instruction in instructions)
    {
        if(ProcessInstruction(part, instruction))
        {
            //Don't process the rest of the instructions if one is successful
            return;
        }
    }
}

bool ProcessInstruction(Part part, string instruction)
{
    if(!instruction.Contains(':'))
    {
        Action func = ConvertStandaloneInstructionToFunction(part, instruction);
        part.Actions.Enqueue(func);
        return true;
    }
    else
    {
        var x = instruction.Split(new char[] {'<','>',':'});
        char comparisonOperator = instruction[1];
        int partValue = part.Values[x[0]];
        int comparisonValue = int.Parse(x[1]);

        if (PassOperation(comparisonOperator, partValue, comparisonValue))
        {
            Action func = ConvertStandaloneInstructionToFunction(part, x[2]);
            part.Actions.Enqueue(func);
            return true;
        }
    }
    return false;
}

bool PassOperation(char comparisonOperator, int v1, int v2)
{
    return comparisonOperator switch
    {
        '<' => v1 < v2,
        '>' => v1 > v2,
        '=' => v1 == v2,
        _ => false,
    };
}

Action ConvertStandaloneInstructionToFunction(Part part, string instruction) =>
instruction switch
{
    "A" => (() => ProcessAcceptance(part)),
    "R" => (() => ProcessRejection(part)),
    _ => (() => ProcessWorkflow(part, instruction))
};

void ProcessRejection(Part part)
{
    part.Actions.Clear();
    part.Rejected = true;
    part.Accepted = false;
}

void ProcessAcceptance(Part part)
{
    part.Actions.Clear();
    part.Rejected = false;
    part.Accepted = true;
}

class Part
{
    public Dictionary<string, int> Values = new();

    public bool Accepted { get; set; } = false;
    public bool Rejected { get; set; } = false;

    public Queue<Action> Actions { get; set; } = new();

    public Part(params string[] input)
    {
        foreach(var v in input)
        {
            var val = v.Split('=');
            Values.Add(val[0], int.Parse(val[1]));
        }
    }

    public int Rating()
    {
        return Values.Values.Sum();
    }
}

class Instructions
{
    public List<string> instructions = new();

    public Instructions(params string[] input)
    {
        instructions = input.ToList();
    }
}