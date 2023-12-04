using System.Text;

string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
    inputFile = "input.txt";
#endif

int result = 0;
string[] input = File.ReadAllLines(inputFile);


List<Round> rounds = new List<Round>();
for (int i = 0; i < input.Length; i++)
{
    Round round = new();
    round.Id = i + 1;

    string line = input[i].Substring(input[i].IndexOf(':'));
    string[] roundData = line.Split(';');

    foreach (string gameData in roundData)
    {
        Game game = new();
        string[] cubeTypes = gameData.Substring(0).Split(",");
        foreach (var cubeTypeData in cubeTypes)
        {
            int cubeNumber = int.Parse(cubeTypeData.Substring(1, cubeTypeData.LastIndexOf(' ')));
            string cubeType = cubeTypeData.Substring(cubeTypeData.LastIndexOf(' ') + 1);
            game.cubeTypes.Add(cubeType, cubeNumber);
        }
        round.games.Add(game);
    }
    rounds.Add(round);
}

Dictionary<string, int> givenBag = new()
            {
                {"red", 12 },
                {"green", 13 },
                {"blue", 14 }
            };

result = rounds.Where(round => round.validateGameAgainstBag(givenBag)).Sum(x => x.Id);

Console.WriteLine(result);
Console.ReadLine();


class Round
{
    public int Id;
    public List<Game> games = new List<Game>();
    public bool validateGameAgainstBag(Dictionary<string, int> bag)
    {
        foreach (var game in games)
        {
            foreach (var cubeType in game.cubeTypes)
            {
                if (!bag.ContainsKey(cubeType.Key))
                {
                    return false;

                }
                if (cubeType.Value > bag[cubeType.Key])
                {
                    return false;
                }
            }
        }
        return true;
    }

    public int minCubeProduct()
    {
        Dictionary<string, int> minCubeTypes = new();
        foreach (Game game in games)
        {
            foreach (var cubeType in game.cubeTypes)
            {
                if (minCubeTypes.ContainsKey(cubeType.Key))
                {
                    if (cubeType.Value > minCubeTypes[cubeType.Key])
                    {
                        minCubeTypes[cubeType.Key] = cubeType.Value;
                    }
                }
                else
                {
                    minCubeTypes.Add(cubeType.Key, cubeType.Value);
                }
            }
        }
        return minCubeTypes.Aggregate(1, (x, y) => x * y.Value);
    }


}

class Game
{
    public Dictionary<string, int> cubeTypes = new();
}