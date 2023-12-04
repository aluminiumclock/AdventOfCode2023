string inputFile;

#if DEBUG
inputFile = "text.txt";
#else
inputFile = "input.txt";
#endif

int result = 0;
string[] input = File.ReadAllLines(inputFile);


Console.WriteLine(result);
Console.ReadLine();
