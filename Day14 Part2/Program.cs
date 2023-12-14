using System.Text;

string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
inputFile = "input.txt";
#endif

string[] input = File.ReadAllLines(inputFile);
int width = input[0].Length;
int height = input.Length;
char[,] platform = new char[width, height];

//load platform
for (int x = 0; x < width; x++)
{
    for (int y = 0; y < height; y++)
    {
        platform[x, y] = input[y][x];
    }
}

Dictionary<string, int> seenPlatforms = new();
int maxCycle = 0;
const int CYCLE_COUNT = 1000000000;//1000000000

//var Origcolumn = Enumerable.Range(0, width)
//                .Select(x => platform[0, x])
//                .ToArray();


//var pColumns = getColumns(platform);
//var column = Enumerable.Range(0, width)
//                .Select(x => platform[7, x])
//                .ToArray();

platform = moveBoulders(platform);
seenPlatforms.Add(PlatformAsString(platform), scorePlatform(platform));

for (int cycles = 1; cycles <= 30; cycles++)
{
    for (int count = 0; count < 4; count++)
    {
        platform = rotatePlatformNegative90Degress(platform);
        platform = moveBoulders(platform);
    }
    maxCycle = cycles;

    //string PlatformString = PlatformAsString(platform);
    //if (seenPlatforms.ContainsKey(PlatformString)) { break; }
    //seenPlatforms.Add(PlatformString, scorePlatform(platform));
    Console.WriteLine(scorePlatform(platform));
}

Console.WriteLine("Score for All Loads So Far");
int AboveEnd = CYCLE_COUNT % (maxCycle - 1);
Console.WriteLine($"Cycle count: {AboveEnd}");


int result = scorePlatform(platform);

Console.WriteLine(maxCycle);
Console.WriteLine(result);
Console.ReadLine();

char[,] moveBoulders(char[,] platform)
{
    int width = platform.GetLength(1);
    int height = platform.GetLength(0);
    for (int i = 0; i < width; i++)
    {
        for (int j = 0; j < height; j++)
        {
            for (int r = 1; r < height; r++)
            {
                if (platform[i, r] == 'O' && platform[i, r - 1] == '.')
                {
                    platform[i, r] = '.';
                    platform[i, r - 1] = 'O';
                }
            }   
        }
    }
    return platform;
}


char[,] rotatePlatformNegative90Degress(char[,] platform)
{
    int width = platform.GetLength(1);
    int height = platform.GetLength(0);
    if (width > height) { throw new ArgumentException(); }

    char[,] newPlatform = new char[width, height];
    for (int i = 0; i < width; i++)
    {
        for (int j = 0; j < height; j++)
        {
            newPlatform[height - 1 - j , i] = platform[i, j];
        }
    }

    return newPlatform;
}

int scorePlatform(char[,] platform)
{
    int width = platform.GetLength(1);
    int height = platform.GetLength(0);
    int score = 0;
    for (int i = 0; i < height; i++)
    {
        for (int j = 0; j < width; j++)
        {
            if (platform[j ,i]  == 'O')
            {
                score += height - i;
            }
        }
    }
    return score;
}

List<char[]> getColumns(char[,] platform)
{
    List<char[]> Columns = new();
    int width = platform.GetLength(0);
    int height = platform.GetLength(1);

    for (int row = 0; row < height; row++)
    {
        var column = Enumerable.Range(0, width)
                        .Select(x => platform[row, x])
                        .ToArray();
        Columns.Add(column);
    }

    return Columns;
}

static string PlatformAsString(char[,] platform)
{
    int width = platform.GetLength(0);
    int height = platform.GetLength(1);

    StringBuilder sb = new StringBuilder();
    for (int i = 0; i < height; i++)
    {
        for (int j = 0; j < width; j++)
        {
            sb.Append(platform[i, j]);
        }
    }
    return sb.ToString();
}


Dictionary<string, int> chars = new();

//put array in a dictionary
char[,] data1 = new char[,] { { 'a', 'b' }, { 'c', 'd' }, { 'e', 'f' }, { 'g', 'h' } };
char[,] data2 = new char[,] { { 'a', 'b' }, { 'c', 'd' }, { 'e', 'f' }, { 'g', 'h' } };


chars.Add(new String(data1.Cast<char>().ToArray()), 1);
//chars.Add(new String(data2.Cast<char>().ToArray()), 2);

int total = chars.Count();

