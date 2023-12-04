using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    public class Day1Trebuchet : IRunnable
    {
        public void Run()
        {
            int sum = 0;
            string[] lines = File.ReadAllLines(@"Day01\CalibrationDocument.txt");

            foreach (string line in lines)
            {
                int firstNumber = line.First(c => char.IsDigit(c)) - '0';
                int lastNumber = line.Last(c => char.IsDigit(c)) - '0';

                sum += firstNumber * 10 + lastNumber;
            }
            Console.WriteLine(sum);
        }
    }
}
