using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
    inputFile = "input.txt";
#endif

List<SpringRow> rows = File.ReadAllLines(inputFile).Select(x => x.Split(' ')).Select(y => new SpringRow(y[0], y[1])).Skip(6).ToList();

int result = 0;
Dictionary<(string, string), int> PermutationCache = new();

//Pre-Populate dictionary with some obvious answers
//for (int i = 0; i < 10; i++)
//{
//    string line = new string('#', i);
//    PermutationCache.Add((line, i.ToString()), 1);
//    PermutationCache.Add((line + '.', i.ToString()), 1);
//    PermutationCache.Add((line + "..", i.ToString()), 1);
//    PermutationCache.Add((line + '?', i.ToString()), 2);
//}

int counter = 0;
int rowTotal = 0;
int row = 1;

foreach(var springRow in rows)
{
    Console.WriteLine("Start:" + springRow.line + ", " + springRow.brokenSpring.Length + ":" + springRow.brokenSpring[0] );
    rowTotal += Permutations(springRow.line, springRow.brokenSpring);
    result += rowTotal;
    Console.WriteLine($"Row: {row++} has {rowTotal} permutations \n");
    rowTotal = 0;
}

Console.WriteLine("Permutations ran to end " + counter + " times.");
Console.WriteLine(result);
//Console.ReadLine();

int Permutations(string line, int[] brokenSprings)
{
    //check if we've already done this lookup and use the value if we have
    if(PermutationCache.TryGetValue((line, string.Join(",",brokenSprings)), out var value))
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
                        && line.Substring(brokenSprings[0])[0] != '#') 
                   ?
            Permutations(line[(brokenSprings[0] + 1)..], brokenSprings[1..]) 
            : 0;

    current = line.Length <= brokenSprings[0] && line.All(x => x == '#' || x == '?') ? 1 : current;

    int skipped = line[0] == '?' ? Permutations(line[1..], brokenSprings) : 0;

    int result = current + skipped;
    PermutationCache.Add((line, string.Join(",", brokenSprings)), result);

    counter++;
    //if (result > 0)
    //{
        Console.WriteLine(line + ", " + brokenSprings.Length + ":" + brokenSprings[0] + " Result:" + result);
    //}
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