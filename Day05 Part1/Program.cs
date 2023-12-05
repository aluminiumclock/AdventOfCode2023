string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
    inputFile = "input.txt";
#endif

Int64 result = 0;
string[] input = File.ReadAllLines(inputFile);

//Setup Maps
string CurrentMap = ""; 
AlmanacMap SeedToSoil = new();
AlmanacMap SoilToFertilizer = new();
AlmanacMap FertilizerToWater = new();
AlmanacMap WaterToLight = new();
AlmanacMap LightToTemperature = new();
AlmanacMap TemperatureToHumidty = new();
AlmanacMap HumidtyToLocation = new();

List<AlmanacMap> AllMaps = new()
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
Int64[] seeds = Array.ConvertAll(seedsText,s => Int64.Parse(s));

//Read Dictionaries
for (Int64 i = 1; i < input.Length; i++)
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
Int64 minLocation = Int64.MaxValue;

foreach (Int64 seed in seeds)
{
    Int64 location =  NavigateThroughAllMaps(seed);
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
    AlmanacMap selectedMap = SelectMap(currentMap);
    
    string[] data = line.Split(' ',StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    Int64 startingValue = Int64.Parse(data[0]);
    Int64 startingKey = Int64.Parse(data[1]);
    Int64 count = Int64.Parse(data[2]);

    selectedMap.mapRanges.Add(new MapRange { StartingKey = startingKey, StartingValue = startingValue, Count = count });
}

AlmanacMap SelectMap(string currentMap) => currentMap switch
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

Int64 NavigateThroughAllMaps(Int64 seed)
{
    foreach(AlmanacMap map in AllMaps)
    {
        seed = map.Navigate(seed);
    }

    return seed;
}

class AlmanacMap
{
    public List<MapRange> mapRanges = new();

    public Int64 Navigate(Int64 seed)
    {
        foreach(MapRange mapRange in mapRanges)
        {
            Int64 rangeResult = mapRange.Navigate(seed);

            if(rangeResult != -1) {
                return rangeResult;
            }
        }

        return seed;
    }
}

class MapRange
{
    public Int64 StartingKey;
    public Int64 StartingValue;
    public Int64 Count;

    public Int64 Navigate(Int64 seed)
    {
        if (seed < StartingKey)
        {
            return -1;
        }

        Int64 difference = seed - StartingKey;
        if (difference < Count)
        {
            return StartingValue + difference;
        }

        return -1;
    }
}