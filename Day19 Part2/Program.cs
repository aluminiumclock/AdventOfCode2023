using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
    inputFile = "input.txt";
#endif

string[] input = File.ReadAllText(inputFile).Replace("\r\n", "\n").Split("\n\n");

Dictionary<string, Instructions> workflows = ParseWorkflows(input[0]);

var resultBands = AcceptedBands(workflows);
Int128 result = 0;
foreach (var resultBand in resultBands)
{
    result += resultBand.XmasVolume;
}

Console.WriteLine(result);

Dictionary<string, Instructions> ParseWorkflows(string v)
{
    var lines = v.Split("\n");
    return lines.Select(x => SplitInstructions(x)).ToDictionary(y => y.Item1, y => y.Item2);
}

(string, Instructions) SplitInstructions(string workflow)
{
    var data = workflow.Split(new char[] { '{', '}', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    return ((string)data[0], new Instructions(data[1..]));
}

List<XMAS_ANSWER> AcceptedBands(Dictionary<string, Instructions> workflows)
{
    var q = new Queue<(XMAS_ANSWER xmas, string workflowAction)>();
    q.Enqueue((new XMAS_ANSWER(), "in"));
    var acceptedBands = new List<XMAS_ANSWER>();

    while(q.Count > 0)
    {
        (var xmas, var workflowAction) = q.Dequeue();
        if(workflowAction == "R")
        {
            continue;
        }
        else if(workflowAction == "A")
        {
            acceptedBands.Add(xmas);
        }
        else if (xmas.IsEmpty)
        {
            continue;
        }
        else
        {
            var instructions = workflows[workflowAction].instructions;
            foreach(var instruction in instructions)
            {
                if (instruction.Contains(":"))
                {
                    var x = instruction.Split(new char[] { '<', '>', ':' });

                    string whatValueToCompare = x[0];
                    char comparisonOperator = instruction[1];
                    int compareAgainst = int.Parse(x[1]);
                    string nextAction = x[2];

                    (XMAS_ANSWER xmas_low, XMAS_ANSWER xmas_high) newXmas = SplitXmas(xmas, compareAgainst, whatValueToCompare, comparisonOperator);
                    
                    if(comparisonOperator == '<')
                    {
                        q.Enqueue((newXmas.xmas_low, nextAction));
                        //update xmas_result passing through the next instructions
                        xmas = newXmas.xmas_high;
                    }
                    else if (comparisonOperator == '>')
                    {
                        q.Enqueue((newXmas.xmas_high, nextAction));
                        //update xmas_result passing through the rest of the workflow
                        xmas = newXmas.xmas_low;
                    }
                }
                else{
                    q.Enqueue((xmas, instruction));
                }
            }
        }
    }

    return acceptedBands;
}

(XMAS_ANSWER, XMAS_ANSWER) SplitXmas(XMAS_ANSWER xmas, int compareAgainst, string whatValueToCompare, char comparisonOperator)
{
    int currentMin = xmas.GetMin(whatValueToCompare);
    int currentMax = xmas.GetMax(whatValueToCompare);

    compareAgainst = comparisonOperator == '<' ? compareAgainst - 1 : compareAgainst;

    XMAS_ANSWER xmas_low = new XMAS_ANSWER(xmas, whatValueToCompare, currentMin, Math.Min(currentMax, compareAgainst));
    XMAS_ANSWER xmas_high = new XMAS_ANSWER(xmas, whatValueToCompare, Math.Max(currentMin, compareAgainst + 1), currentMax); 

    return (xmas_low, xmas_high);
}

record XMAS_ANSWER
{
    public (int start, int end) X;
    public (int start, int end) M;
    public (int start, int end) A;
    public (int start, int end) S;

    public XMAS_ANSWER()
    {
        X = (1, 4000);
        M = (1, 4000);
        A = (1, 4000);
        S = (1, 4000);
    }

    public XMAS_ANSWER(XMAS_ANSWER xmas, string value, int start, int end)
    {
        X = xmas.X;
        M = xmas.M;
        A = xmas.A;
        S = xmas.S;

        switch (value)
        {
            case "x" : X = (start, end); break;
            case "m" : M = (start, end); break;
            case "a" : A = (start, end); break;
            case "s" : S = (start, end); break;
            default: throw new ArgumentException();
        };
    }

    public int GetMin(string value) =>
    value switch
    {
        "x" => X.start,
        "m" => M.start,
        "a" => A.start,
        "s" => S.start,
        _ => throw new ArgumentException()
    };


    public int GetMax(string value) =>
    value switch
    {
        "x" => X.end,
        "m" => M.end,
        "a" => A.end,
        "s" => S.end,
        _ => throw new ArgumentException()
    };

    public bool IsEmpty {
        get
        {
            return (X.start > X.end
                || M.start > M.end
                || A.start > A.end
                || S.start > S.end);
        }
    }

    public long XmasVolume
    {
        get
        {
            return ((long)(X.end - X.start + 1)
                * (long)(M.end - M.start + 1)
                * (long)(A.end - A.start + 1)
                * (long)(S.end - S.start + 1));
        }
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