namespace AdventOfCode2023
{
    public class Program
    {
        private static void Main(string[] args)
        {
            IRunnable AppToRun = new SecondGearRatio();
            AppToRun.Run();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}