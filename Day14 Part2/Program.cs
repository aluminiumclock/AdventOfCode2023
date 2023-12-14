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

//Create variables for cycle
Dictionary<string, int> seenPlatforms = new();
Dictionary<int, int> scores = new();

int period = 0;
int start = 0;
int CYCLE_COUNT = 1000000000;

//Add Initial state too dictionary
seenPlatforms.Add(PlatformAsString(platform), 0);
scores.Add(0, scorePlatform(platform));
Console.WriteLine("0:" + scorePlatform(platform));

for (int cycles = 1; cycles <= CYCLE_COUNT; cycles++)
{

    platform = runCycle(platform);

    string PlatformString = PlatformAsString(platform);
    if (seenPlatforms.ContainsKey(PlatformString))
    {
        start = seenPlatforms[PlatformString];
        period = cycles - start;
        break;
    }
    seenPlatforms.Add(PlatformString, cycles);
    scores.Add(cycles, scorePlatform(platform));
    Console.WriteLine($"{cycles}: {scorePlatform(platform)}");
}

//calculate witch is the last platform
int finalPlatform = start + (CYCLE_COUNT - start) % period;
int result = scores[finalPlatform];

Console.WriteLine(result);
Console.ReadLine();

char[,] runCycle(char[,] platform)
{
    char[,] newPlatform = moveBoulders(platform);
    for (int count = 0; count < 3; count++)
    {
        newPlatform = rotatePlatform90Degress(newPlatform);
        newPlatform = moveBoulders(newPlatform);
    }
    newPlatform = rotatePlatform90Degress(newPlatform);
    return newPlatform;
}

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


char[,] rotatePlatform90Degress(char[,] platform)
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

static void PrintPlatform(char[,] platform)
{
    int width = platform.GetLength(0);
    int height = platform.GetLength(1);

    StringBuilder sb = new StringBuilder();
    for (int j = 0; j < width; j++) 
    {
        for (int i = 0; i < height; i++)
        {
            sb.Append(platform[i, j]);
        }
        sb.AppendLine();
    }
    sb.AppendLine();
    Console.WriteLine(sb.ToString());
}

static string PlatformAsString(char[,] platform)
{
    int width = platform.GetLength(0);
    int height = platform.GetLength(1);

    StringBuilder sb = new StringBuilder();
    for (int j = 0; j < width; j++) 
    {
        for (int i = 0; i < height; i++)
        {
            sb.Append(platform[i, j]);
        }
    }
    return sb.ToString();
}
