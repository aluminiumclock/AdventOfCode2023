using System.Text;

string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
inputFile = "input.txt";
#endif

string input = File.ReadAllText(inputFile);
input.Replace("\n", "");
input.Replace("\r", "");
var initialisationKeys = input.Split(',');


Dictionary<int, Box> Boxes= new Dictionary<int, Box>();
for (int i = 0; i < 256; i++)
{
    Boxes[i] = new Box();
}

foreach (var key in initialisationKeys)
{
    //Removing Lenses
    if (key[^1] ==  '-')
    {
        string lensId = key.Substring(0,key.Length - 1);
        int boxId = LavaHash(lensId);
        Box curentBox = Boxes[boxId];
        curentBox.Remove(lensId);
        continue;
    }

    if (key.Contains('='))
    {
        string[] newLensData = key.Split('=');
        string lensId = newLensData[0];
        int focalStrength = int.Parse(newLensData[1]);
        int boxId = LavaHash(lensId);
        Box currentBox = Boxes[boxId];
        currentBox.Add(lensId, focalStrength);
    }
}


int result = 0;
for(int i = 0; i < Boxes.Count; i++)
{
    Box currentBox = Boxes[i];
    if (!currentBox.IsEmpty)
    {
        int boxTotal = (i + 1) * currentBox.LensStrength();
        //Console.WriteLine($"The total for box {i + 1} is {boxTotal}");
        result += boxTotal;
    }
}



Console.WriteLine(result);
Console.ReadLine();

int LavaHash(string key)
{
    int currentValue = 0;
    foreach (char c in key)
    {
        currentValue += (int)c;
        currentValue *= 17;
        currentValue = currentValue % 256;
    }
    return currentValue;
}


class Box
{
    public List<Lens> Lenses = new();
    public bool IsEmpty
    {
        get { return Lenses.Count == 0;}
    }

    public void Remove(string lensId)
    {
        int lensIndex = -1;
        for(int i = 0; i < Lenses.Count; i++)
        {
            if (Lenses[i].Id == lensId)
            {
                lensIndex = i;
            }
        }

        if(lensIndex != -1)
        {
            Lenses.RemoveAt(lensIndex);
        }
    }

    public void Add(string lensId, int focalStrength)
    {
        for (int i = 0; i < Lenses.Count; i++)
        {
            if (Lenses[i].Id == lensId)
            {
                Lenses[i].FocalStrength = focalStrength;
                return;
            }
        }

        Lenses.Add(new Lens(lensId, focalStrength));
    }

    public int LensStrength()
    {
        int total = 0;

        for (int i = 0; i < Lenses.Count; i++)
        {
            total += (i + 1) * Lenses[i].FocalStrength;
        }

        return total;
    }
}

class Lens
{
    public Lens(string id, int focalStrength)
    {
        Id = id;
        FocalStrength = focalStrength;
    }

    public string Id { get; set; }
    public int FocalStrength { get; set; }
}


