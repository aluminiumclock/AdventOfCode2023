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
List<SeedRange> seeds = new();
string[] seedsText = input[0].Substring(7).Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

for(int i = 0; i * 2 <= seedsText.Length; i += 2)
{
    Int64 startingSeed = Int64.Parse(seedsText[i]);
    Int64 count = Int64.Parse(seedsText[i + 1]);
    seeds.Add(new SeedRange { StartingSeed = startingSeed, Count = count});
}

//Read Maps
for (Int64 i = 1; i < input.Length; i++)
{
    string line = input[i];

    if (String.IsNullOrWhiteSpace(line))
    {
        CurrentMap = "";
    }

    if (CurrentMap == "")
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

foreach (SeedRange seedRange in seeds)
{
    Console.WriteLine($"StartingSeed: {seedRange.StartingSeed} For:{seedRange.Count}"); 
    Int64 location = MinLocationViaMaps(seedRange);
    if (location < minLocation)
    {
        minLocation = location;
    }
}

result = minLocation;
Console.WriteLine(result);
Console.ReadLine();

#region Methods

void AddToMap(string line, string currentMap)
{
    AlmanacMap selectedMap = SelectMap(currentMap);

    string[] data = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
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
    foreach (AlmanacMap map in AllMaps)
    {
        seed = map.Navigate(seed);
    }

    return seed;
}

Int64 MinLocationViaMaps(SeedRange seedRange)
{

    Int64 minLocation = Int64.MaxValue;
    for(Int64 i = seedRange.StartingSeed; i < seedRange.StartingSeed + seedRange.Count; i++)
    {
        Int64 Location = NavigateThroughAllMaps(i);
        if(Location < minLocation)
        {
            minLocation = Location;
        }
    }

    return minLocation;
}
#endregion

#region Classes
class AlmanacMap
{
    public List<MapRange> mapRanges = new();

    public Int64 Navigate(Int64 seed)
    {
        foreach (MapRange mapRange in mapRanges)
        {
            Int64 rangeResult = mapRange.Navigate(seed);

            if (rangeResult != -1)
            {
                return rangeResult;
            }
        }

        return seed;
    }

    public Int64 MinLocationInSeedRange(SeedRange seedRange)
    {
        Int64 minLocation = Int64.MaxValue;
        foreach(MapRange mapRange in mapRanges)
        {
            Int64 mapRangeMinLocation = mapRange.MinLocationInSeedRange(seedRange);
            if (mapRangeMinLocation < minLocation)
            {
                minLocation = mapRangeMinLocation;
            }
        }

        return minLocation;
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

    public long MinLocationInSeedRange(SeedRange seedRange)
    {
        throw new NotImplementedException();
    }
}

class SeedRange
{
    public Int64 StartingSeed;
    public Int64 Count;
}
#endregion