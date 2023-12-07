
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
List<NumberRange> seeds = new();
string[] seedsText = input[0].Substring(7).Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

for(int i = 0; i * 2 <= seedsText.Length; i += 2)
{
    Int64 startingSeed = Int64.Parse(seedsText[i]);
    Int64 count = Int64.Parse(seedsText[i + 1]);
    seeds.Add(new NumberRange { InitialValue = startingSeed, Count = count});
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

foreach (NumberRange seedRange in seeds)
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

    selectedMap.SubMaps.Add(new MapRange { InitialValue = initialValue, MappedTo = mappedTo, Count = count });
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

Int64 MinLocationViaMaps(NumberRange seedRange)
{
    List<NumberRange> seeds = new List<NumberRange>
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
    public List<MapRange> SubMaps = new();

    public List<NumberRange> NavigateRanges(List<NumberRange> ranges)
    {
        List<NumberRange> resultingNumberRanges = new();

        foreach(NumberRange range in ranges)
        {
            List<NumberRange> mappedRange = SubMaps.SelectMany(x => x.NavigateRange(range)).ToList();

            //Adding missing ranges
            Int64 StartingValue = range.InitialValue;
            foreach (NumberRange numberRange in mappedRange.OrderBy(x => x.InitialValue).ToList())
            {
                if (StartingValue != numberRange.InitialValue)
                {
                    mappedRange.Add(new NumberRange { InitialValue = StartingValue, Count = numberRange.InitialValue - StartingValue -1 });
                }
                StartingValue = numberRange.MaxValue + 1;
            }
            if (StartingValue < range.MaxValue)
            {
                mappedRange.Add(new NumberRange { InitialValue = StartingValue, Count = range.MaxValue - StartingValue });
            }

            resultingNumberRanges.AddRange(mappedRange);
        }

        resultingNumberRanges.ForEach(x => x.ApplyOffset());
        return resultingNumberRanges;
    }
}

class MapRange
{
    public Int64 InitialValue;
    public Int64 MappedTo;
    public Int64 Count;
    public Int64 MaxValue => InitialValue + Count;
    public Int64 OffSet => MappedTo - InitialValue;
    public List<NumberRange> NavigateRange(NumberRange range)
    { 
        List<NumberRange> mappedRanges = new();
 
        if (Between(range.InitialValue, this.InitialValue,this.MaxValue) || Between(range.MaxValue, this.InitialValue, this.MaxValue) || Between(this.InitialValue, range.InitialValue, range.MaxValue))
        {
            Int64 newInitialValue = Math.Max(range.InitialValue,InitialValue);
            Int64 newCount = Math.Min(range.MaxValue,this.MaxValue) - newInitialValue;
            mappedRanges.Add(new NumberRange { InitialValue = newInitialValue, Count = newCount, OffSet = this.OffSet });
        }

        return mappedRanges;
    }

    private bool Between(long startingSeed, long startingKey, long maxKey)
    {
        return (startingSeed >= startingKey && startingSeed <= maxKey);
    }
}

class NumberRange
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