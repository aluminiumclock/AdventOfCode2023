string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
    inputFile = "input.txt";
#endif

int result = 0;
string[] input = File.ReadAllLines(inputFile);
foreach (string line in input)
{
    Console.WriteLine(line);
}

Console.WriteLine(result);
Console.ReadLine();
