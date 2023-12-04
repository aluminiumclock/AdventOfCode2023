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
    int firstNumber = line.First(c => char.IsDigit(c)) - '0';
    int lastNumber = line.Last(c => char.IsDigit(c)) - '0';

    result += firstNumber * 10 + lastNumber;
}

Console.WriteLine(result);
Console.ReadLine();
