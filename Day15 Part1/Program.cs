using System.Text;

string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
    inputFile = "input.txt";
#endif

int result = 0;
string input = File.ReadAllText(inputFile);
input.Replace("\n", "");
input.Replace("\r", "");

var initialisationKeys = input.Split(',');


foreach (var key in initialisationKeys)
{
    result += LavaHash(key);
}


Console.WriteLine(result);
Console.ReadLine();

int LavaHash(string key)
{
    int currentValue = 0;
    foreach (char c in key)
    {
        currentValue += (int)c;
        currentValue *= 17;
        currentValue = currentValue % 256;
    }
    return currentValue;
}
