using System.ComponentModel;

string inputFile;
#if DEBUG
    inputFile = "test.txt";
#else
inputFile = "input.txt";
#endif

const int BUTTON_PRESS_COUNT = 1000;

Dictionary<string, CommunicationModule> communicationModules = File.ReadAllLines(inputFile).
    Select(x => x.Split(new string[] { "->", "," }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)).
    ToDictionary(y => y[0] == "broadcaster" ? y[0] : y[0][1..], z => CommunicationModuleFactory.CreateCommunicationModule(z));

SetupConjunctionModules(communicationModules);


//the button will be pressed multiples times and might well create a cycle (we could implment cycle detection here
for (int i = 0; i < BUTTON_PRESS_COUNT; i++)
{
    PressStartButton(communicationModules);
    //MulitplyPulses(communicationModules);
    //communicationModules.Values.ToList().ForEach(x => x.ResetPulseCount());
}

int result = MulitplyPulses(communicationModules);

Console.WriteLine(result);
Console.ReadLine();


void PressStartButton(Dictionary<string, CommunicationModule> communicationModules)
{
    Queue<Pulse> pulses = new();
    pulses.Enqueue(new Pulse("button", "broadcaster", PulseType.LowPulse));

    while (pulses.TryDequeue(out var pulse))
    {
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

}

void SetupConjunctionModules(Dictionary<string, CommunicationModule> communicationModules)
{
    var conjuncitonModules = communicationModules.Where(x => x.Value.GetType() == typeof(ConjunctionModule));

    foreach (KeyValuePair<string, CommunicationModule> conjuectionModule in conjuncitonModules)
    {
        var inputModules = communicationModules.Where(x => x.Value.DestinationModules.Contains(conjuectionModule.Key)).ToDictionary(x => x.Key, x => PulseType.LowPulse);
        conjuectionModule.Value.InputModules = inputModules;
    }
}

static int MulitplyPulses(Dictionary<string, CommunicationModule> communicationModules)
{


    //Sum the high & low pulses and multiply them togther
    return communicationModules.Values.ToList()
                    .GroupBy(x => 1)
                    .Select(grp => new
                    {
                        LowPulses = grp.Sum(x => x.LowPulses),
                        HighPulses = grp.Sum(x => x.HighPulses)
                    })
                    .Select(x => x.LowPulses * x.HighPulses)
                    .First();
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
    public int LowPulses { get; private set; }
    public int HighPulses { get; private set; }

    protected CommunicationModule(List<string> destinationModules)
    {
        DestinationModules = destinationModules;
    }

    public void ResetPulseCount()
    {
        LowPulses = HighPulses = 0;
    }

    protected void RecordPulse(PulseType type)
    {
        if (type == PulseType.LowPulse) LowPulses++;
        else if (type == PulseType.HighPulse) HighPulses++;
    }

    public IEnumerable<Pulse> RecievePulse(Pulse pulse)
    {
        RecordPulse(pulse.PulseType);
        return SendPulses(pulse);
    }

    protected abstract IEnumerable<Pulse> SendPulses(Pulse pulse);
}

class FlipFlopModule : CommunicationModule
{
    public FlipFlopModule(List<string> destinationModules) : base(destinationModules) { }

    private bool Active = false;

    protected override IEnumerable<Pulse> SendPulses(Pulse pulse)
    {
        if (pulse.PulseType == PulseType.HighPulse)
        {
            return Array.Empty<Pulse>();
        }

        var pulsesToSend = Active ? DestinationModules.Select(x => new Pulse(pulse.RecieverId, x, PulseType.LowPulse)) :
                                        DestinationModules.Select(x => new Pulse(pulse.RecieverId, x, PulseType.HighPulse));
        Active = !Active;

        return pulsesToSend;
    }
}

class ConjunctionModule : CommunicationModule
{
    public ConjunctionModule(List<string> destinationModules) : base(destinationModules) { }

    protected override IEnumerable<Pulse> SendPulses(Pulse pulse)
    {
        InputModules[pulse.SenderId] = pulse.PulseType;

        IEnumerable<Pulse> pulsesToSend = InputModules.Values.All(x => x == PulseType.HighPulse) ?
                DestinationModules.Select(x => new Pulse(pulse.RecieverId, x, PulseType.LowPulse)) :
                DestinationModules.Select(x => new Pulse(pulse.RecieverId, x, PulseType.HighPulse));

        return pulsesToSend;
    }
}

class BroadcastModule : CommunicationModule
{
    public BroadcastModule(List<string> destinationModules) : base(destinationModules) { }

    protected override IEnumerable<Pulse> SendPulses(Pulse pulse)
    {
        return DestinationModules.Select(x => new Pulse(pulse.RecieverId, x, PulseType.LowPulse));
    }
}

class TestingModule : CommunicationModule
{
    public TestingModule() : base(new List<string>()) { }

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