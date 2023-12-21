using System.ComponentModel;

string inputFile = "input.txt";

Dictionary<string, CommunicationModule> communicationModules = File.ReadAllLines(inputFile).
    Select(x => x.Split(new string[] { "->", "," }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)).
    ToDictionary(y => y[0] == "broadcaster" ? y[0] : y[0][1..], z => CommunicationModuleFactory.CreateCommunicationModule(z));
LinkConjunctionModulesInputs(communicationModules);

string goal = "rx";
string nodeLeadingToGoal = communicationModules.Where(x => x.Value.DestinationModules.Contains(goal)).Single().Key;
var nodes2removedFromGoal = communicationModules.Where(x => x.Value.DestinationModules.Contains(nodeLeadingToGoal)).ToDictionary(x => x.Key, x => 0);

foreach(var node in nodes2removedFromGoal)
{
    var nodeType = communicationModules[node.Key].GetType();
    if(nodeType == typeof(FlipFlopModule))
    {
        nodes2removedFromGoal[node.Key] = 2;
        continue;
    }

    if(nodeType == typeof(BroadcastModule))
    {
        nodes2removedFromGoal[node.Key] = 1;
        continue;
    }

    if(nodeType == typeof(ConjunctionModule))
    {
        nodes2removedFromGoal[node.Key] = PressesUntilPulseTypeSent(communicationModules, node.Key, nodeLeadingToGoal, PulseType.HighPulse);
        continue;
    }
    throw new NotImplementedException();
}


var cycles = nodes2removedFromGoal.Values.ToList();
long result = LowestCommonMultiple(cycles);
Console.WriteLine(result);



int PressesUntilPulseTypeSent(Dictionary<string, CommunicationModule> communicationModules, string senderId, string recieverid, PulseType pulseType)
{
    int buttonPresses = 1;
    communicationModules.Values.ToList().ForEach(x => x.Reset());
    //We know an answer exists so I've omitted any checks for loops
    while (PressStartButton(communicationModules, senderId, recieverid, pulseType))
    {
        buttonPresses++;
    }
    return buttonPresses;
}



bool PressStartButton(Dictionary<string, CommunicationModule> communicationModules, string senderId, string recieverId, PulseType pulseType)
{
    Queue<Pulse> pulses = new();
    pulses.Enqueue(new Pulse("button", "broadcaster", PulseType.LowPulse));

    while (pulses.TryDequeue(out var pulse))
    {
        if (pulse.RecieverId == recieverId)
        {
            if(pulse.PulseType == pulseType && pulse.SenderId == senderId)
            {
                return false;
            }
        }

        if (!communicationModules.ContainsKey(pulse.RecieverId))
        {
            communicationModules.Add(pulse.RecieverId, new TestingModule());
        }
        var reciever = communicationModules[pulse.RecieverId];

        foreach (var pulseToSend in reciever.RecievePulse(pulse))
        {
            pulses.Enqueue(pulseToSend);
        }
    }

    return true;
}

void LinkConjunctionModulesInputs(Dictionary<string, CommunicationModule> communicationModules)
{
    var conjuncitonModules = communicationModules.Where(x => x.Value.GetType() == typeof(ConjunctionModule));

    foreach (KeyValuePair<string, CommunicationModule> conjuectionModule in conjuncitonModules)
    {
        var inputModules = communicationModules.Where(x => x.Value.DestinationModules.Contains(conjuectionModule.Key)).ToDictionary(x => x.Key, x => PulseType.LowPulse);
        conjuectionModule.Value.InputModules = inputModules;
    }
}


long LowestCommonMultiple(List<int> numbers)
{
    return numbers.Select(x => (long)x).Aggregate((x, y) => ((x * y) / GreatestCommonDivisor(x, y)));
}

//credit for this method goes to https://stackoverflow.com/questions/18541832/c-sharp-find-the-greatest-common-divisor
long GreatestCommonDivisor(long x, long y)
{
    while (x != 0 && y != 0)
    {
        if (x > y)
            x %= y;
        else
            y %= x;
    }

    return x | y;
}

record Pulse(string SenderId, string RecieverId, PulseType PulseType);

enum PulseType
{
    LowPulse,
    HighPulse
}

enum ModuleType
{
    [Description("Flip-Flop")]
    FlipFlop,
    Conjunction,
    Broadcaster
}

abstract class CommunicationModule
{
    public List<string> DestinationModules { get; protected set; } = new();
    public Dictionary<string, PulseType> InputModules = new();

    protected CommunicationModule(List<string> destinationModules)
    {
        DestinationModules = destinationModules;
    }

    public IEnumerable<Pulse> RecievePulse(Pulse pulse)
    {
        return SendPulses(pulse);
    }

    public abstract void Reset();

    protected abstract IEnumerable<Pulse> SendPulses(Pulse pulse);
}

class FlipFlopModule : CommunicationModule
{
    public FlipFlopModule(List<string> destinationModules) : base(destinationModules) { }

    private bool Active = false;
    public override void Reset()
    {
        Active = false;
    }

    protected override IEnumerable<Pulse> SendPulses(Pulse pulse)
    {
        if (pulse.PulseType == PulseType.HighPulse)
        {
            return Array.Empty<Pulse>();
        }

        // if module on send low pulses
        var pulsesToSend = Active ? DestinationModules.Select(x => new Pulse(pulse.RecieverId, x, PulseType.LowPulse)) :
                                        DestinationModules.Select(x => new Pulse(pulse.RecieverId, x, PulseType.HighPulse));
        Active = !Active;

        return pulsesToSend;
    }

}

class ConjunctionModule : CommunicationModule
{
    public ConjunctionModule(List<string> destinationModules) : base(destinationModules) { }

    public override void Reset()
    {
        InputModules = InputModules.ToDictionary(x => x.Key, x => PulseType.LowPulse);
    }

    protected override IEnumerable<Pulse> SendPulses(Pulse pulse)
    {
        InputModules[pulse.SenderId] = pulse.PulseType;

        if(InputModules.Values.All(x => x == PulseType.HighPulse))
        {
            return DestinationModules.Select(x => new Pulse(pulse.RecieverId, x, PulseType.LowPulse));
        }
        else
        {
            return DestinationModules.Select(x => new Pulse(pulse.RecieverId, x, PulseType.HighPulse));
        }
    }
}

class BroadcastModule : CommunicationModule
{
    public BroadcastModule(List<string> destinationModules) : base(destinationModules) { }

    public override void Reset() { }

    protected override IEnumerable<Pulse> SendPulses(Pulse pulse)
    {
        return DestinationModules.Select(x => new Pulse(pulse.RecieverId, x, PulseType.LowPulse));
    }
}

class TestingModule : CommunicationModule
{
    public TestingModule() : base(new List<string>()) { }

    public override void Reset() { }
    protected override IEnumerable<Pulse> SendPulses(Pulse pulse)
    {
        return Array.Empty<Pulse>();
    }
}

static class CommunicationModuleFactory
{
    public static CommunicationModule CreateCommunicationModule(string[] input)
    {
        List<string> destinationModules = input[1..].ToList();
        ModuleType moduleType = ConvertToModule(input[0]);

        return moduleType switch
        {
            ModuleType.Broadcaster => new BroadcastModule(destinationModules),
            ModuleType.FlipFlop => new FlipFlopModule(destinationModules),
            ModuleType.Conjunction => new ConjunctionModule(destinationModules),
            _ => throw new ArgumentException("ModuleType not setup")
        };
    }

    private static ModuleType ConvertToModule(string moduletype)
    {
        if (moduletype == "broadcaster") return ModuleType.Broadcaster;
        else if (moduletype[0] == '%') return ModuleType.FlipFlop;
        else if (moduletype[0] == '&') return ModuleType.Conjunction;
        throw new ArgumentException("can't find communication module of type " + moduletype);
    }
}


