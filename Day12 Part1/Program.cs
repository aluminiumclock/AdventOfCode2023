using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
    inputFile = "input.txt";
#endif

List<SpringRow> rows = File.ReadAllLines(inputFile).Select(x => x.Split(' ')).Select(y => new SpringRow(y[0], y[1])).Take(1).ToList();

int result = 0;
Dictionary<(string, int[]), int> PermutationCache = new();
int counter = 0;

foreach(var springRow in rows)
{
    result += Permutations(springRow.line, springRow.brokenSpring);
}

Console.WriteLine(counter);
Console.WriteLine(result);
Console.ReadLine();

int Permutations(string line, int[] brokenSprings)
{
    //check if we've already done this lookup
    if(PermutationCache.TryGetValue((line, brokenSprings), out var value))
    {
        return value;
    }

    if(brokenSprings.Length == 0)
    {
        return line.All(x => x != '#') ? 1 : 0;
    }
    if (line.Length < brokenSprings.Sum())
    {
        return 0;
    }
    if (line[0] == '.')
    {
        return Permutations(line[1..], brokenSprings);
    } 
    //choose to go with current number or skip
    int current = (line.Substring(0, brokenSprings[0]).All(x => x != '.') 
                        && line.Length > brokenSprings[0] 
                        && line.Substring(brokenSprings[0])[0] != '#'
                    || line.Length <= brokenSprings[0]) ?
        Permutations(line[brokenSprings[0]..], brokenSprings[1..])
        : 0;


    int skipped = line[0] == '?' ? Permutations(line[1..], brokenSprings) : 0;

    int result = current + skipped;
    PermutationCache.Add((line, brokenSprings), result);

    counter++;
    return result;
}

class SpringRow
{
    public string line;
    public int[] brokenSpring;

    public SpringRow(string inputLine, string brokenSprings)
    {
        line = inputLine;
        brokenSpring = brokenSprings.Split(',').Select(x => int.Parse(x)).ToArray();
    }
}