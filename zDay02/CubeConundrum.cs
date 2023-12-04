using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    public class CubeConundrum : IRunnable
    {
        public void Run()
        {
            String[] lines = File.ReadAllLines(@"Day02\input.txt");

            List<Round> rounds = new List<Round>();
            for (int i = 0; i < lines.Length; i++)
            {
                Round round = new();
                round.Id = i + 1;

                string line = lines[i].Substring(lines[i].IndexOf(':'));
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

            //int total = rounds.Where(round => round.validateGameAgainstBag(givenBag)).Sum(x => x.Id);
            int total = rounds.Select(x => x.minCubeProduct()).Sum();
            Console.WriteLine(total);
        }

        private class Round
        {
            public int Id;
            public List<Game> games = new List<Game>();
            public bool validateGameAgainstBag(Dictionary<string,int> bag)
            {
                foreach(var game in games)
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
                foreach(Game game in games)
                {
                    foreach(var cubeType in game.cubeTypes)
                    {
                        if(minCubeTypes.ContainsKey(cubeType.Key))
                        {
                            if(cubeType.Value > minCubeTypes[cubeType.Key])
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

        private class Game
        {
            public Dictionary<string, int> cubeTypes = new();
        }
    }
}
