string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
    inputFile = "input.txt";
#endif

int result = 0;
string[] input = File.ReadAllLines(inputFile);

//Setup Maps
string CurrentMap = ""; 
Dictionary<int, int> SeedToSoil = new Dictionary<int, int>();
Dictionary<int, int> SoilToFertilizer = new Dictionary<int, int>();
Dictionary<int, int> FertilizerToWater = new Dictionary<int, int>();
Dictionary<int, int> WaterToLight = new Dictionary<int, int>();
Dictionary<int, int> LightToTemperature = new Dictionary<int, int>();
Dictionary<int, int> TemperatureToHumidty = new Dictionary<int, int>();
Dictionary<int, int> HumidtyToLocation = new Dictionary<int, int>();

List<Dictionary<int, int>> AllMaps = new()
{
    SeedToSoil,
    SoilToFertilizer,
    FertilizerToWater,
    WaterToLight,
    LightToTemperature,
    TemperatureToHumidty,
    HumidtyToLocation
};

//Read Seeds
string[] seedsText = input[0].Substring(7).Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
int[] seeds = Array.ConvertAll(seedsText,s => int.Parse(s));

//Read Dictionaries
for (int i = 1; i < input.Length; i++)
{
    string line = input[i];

    if (String.IsNullOrWhiteSpace(line))
    {
        CurrentMap = "";
    }

    if(CurrentMap == "")
    {
        CurrentMap = line;
    }
    else
    {
        AddToMap(line, CurrentMap);
    }
}

//Find min location
int minLocation = int.MaxValue;

foreach (int seed in seeds)
{
    int location =  MapThroughAllMaps(seed);
    if(location < minLocation)
    {
        minLocation = location;
    }
}

result = minLocation;
Console.WriteLine(result);
Console.ReadLine();



void AddToMap(string line, string currentMap)
{
    Dictionary<int, int> selectedMap = SelectMap(currentMap);
    
    string[] data = line.Split(' ',StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    int StartingValue = int.Parse(data[0]);
    int StartingKey = int.Parse(data[1]);
    int count = int.Parse(data[2]);

    for (int i = 0; i < count; i++)
    {
        selectedMap.Add(StartingKey + i, StartingValue + i);
    }
}

Dictionary<int, int> SelectMap(string currentMap) => currentMap switch
{
    "seed-to-soil map:" => SeedToSoil,
    "soil-to-fertilizer map:" => SoilToFertilizer,
    "fertilizer-to-water map:" => FertilizerToWater,
    "water-to-light map:" => WaterToLight,
    "light-to-temperature map:" => LightToTemperature,
    "temperature-to-humidity map:" => TemperatureToHumidty,
    "humidity-to-location map:" => HumidtyToLocation,
    _ => throw new ArgumentOutOfRangeException(nameof(currentMap), $"Not an known map: {currentMap}")
};

int MapThroughAllMaps(int seed)
{
    foreach(Dictionary<int,int> map in AllMaps)
    {
        if (map.ContainsKey(seed))
        {
            seed = map[seed];
        }
    }

    return seed;
}