
string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
inputFile = "input.txt";
#endif

Int64 result = 0;
string[] input = File.ReadAllLines(inputFile);

//Setup Maps
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
    seeds.Add(new SeedRange { InitialValue = startingSeed, Count = count});
}

//Read Maps
string CurrentMap = "";
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
    Int64 mappedTo = Int64.Parse(data[0]);
    Int64 initialValue = Int64.Parse(data[1]);
    Int64 count = Int64.Parse(data[2])-1;

    selectedMap.mapRanges.Add(new MapRange { InitialValue = initialValue, MappedTo = mappedTo, Count = count });
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

Int64 MinLocationViaMaps(SeedRange seedRange)
{
    List<SeedRange> seeds = new List<SeedRange>
    {
        seedRange
    };

    foreach(AlmanacMap map in AllMaps)
    {
        seeds = map.NavigateRanges(seeds);
    }

    return seeds.Select(x => x.InitialValue).Min();
}
#endregion

#region Classes
class AlmanacMap
{
    public List<MapRange> mapRanges = new();

    public List<SeedRange> NavigateRanges(List<SeedRange> seeds)
    {
        List<SeedRange> mappedSeeds = new();
        foreach(SeedRange seed in seeds)
        {
            List<SeedRange> mapRangeSeeds = mapRanges.SelectMany(x => x.NavigateRange(seed)).ToList();

            //Adding missing seed ranges
            Int64 SStart = seed.InitialValue;
            foreach (SeedRange seedRange in mapRangeSeeds.OrderBy(x => x.InitialValue).ToList())
            {
                if (SStart != seedRange.InitialValue)
                {
                    mapRangeSeeds.Add(new SeedRange { InitialValue = SStart, Count = seedRange.InitialValue - SStart -1 });
                }
                SStart = seedRange.MaxValue + 1;
            }
            if (SStart < seed.MaxValue)
            {
                mapRangeSeeds.Add(new SeedRange { InitialValue = SStart, Count = seed.MaxValue - SStart });
            }

            mappedSeeds.AddRange(mapRangeSeeds);
        }

        mappedSeeds.ForEach(x => x.ApplyOffset());
        return mappedSeeds;
    }
}

class MapRange
{
    public Int64 InitialValue;
    public Int64 MappedTo;
    public Int64 Count;
    public Int64 MaxValue => InitialValue + Count;
    public Int64 OffSet => MappedTo - InitialValue;
    public List<SeedRange> NavigateRange(SeedRange seed)
    { 
        List<SeedRange> mappedSeeds = new();
 
        if (Between(seed.InitialValue, this.InitialValue,this.MaxValue) || Between(seed.MaxValue, this.InitialValue, this.MaxValue) || Between(this.InitialValue, seed.InitialValue, seed.MaxValue))
        {
            Int64 newStartingKey = Math.Max(seed.InitialValue,InitialValue);
            Int64 newCount = Math.Min(seed.MaxValue,this.MaxValue) - newStartingKey;
            mappedSeeds.Add(new SeedRange { InitialValue = newStartingKey, Count = newCount, OffSet = this.OffSet });
        }

        return mappedSeeds;
    }

    private bool Between(long startingSeed, long startingKey, long maxKey)
    {
        return (startingSeed >= startingKey && startingSeed <= maxKey);
    }
}

class SeedRange
{
    public Int64 InitialValue;
    public Int64 Count;
    public Int64 OffSet = 0;
    public Int64 MaxValue => InitialValue + Count;

    public void ApplyOffset()
    {
        InitialValue = InitialValue + OffSet;
        OffSet = 0;
    }
}
#endregion