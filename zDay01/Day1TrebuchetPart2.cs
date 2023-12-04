using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    public class Day1TrebuchetPart2 : IRunnable
    {
        public void Run()
        {
            Dictionary<string, int> stringDigits = new()
            {
                {"one", 1 },
                {"two", 2 },
                {"three", 3 },
                {"four", 4 },
                {"five", 5 },
                {"six", 6 },
                {"seven", 7 },
                {"eight", 8 },
                {"nine", 9 },
            };

            for (int i = 1; i <= 9; i++)
            {
                stringDigits.Add(i.ToString(), i);
            }

            int sum = 0;
            string[] lines = File.ReadAllLines(@"Day01\CalibrationDocument.txt");

            foreach (string line in lines)
            {
                int firstNumber = 0;
                int firstIndex = line.Length+1;
                int lastNumber = 0;
                int lastIndex = -1;
                
                foreach(var digit in stringDigits)
                {
                    int index = line.IndexOf(digit.Key);

                    if(index == -1)
                    {
                        continue;
                    }

                    if(index < firstIndex)
                    {
                        firstIndex = index;
                        firstNumber = digit.Value;
                    }

                    index = line.LastIndexOf(digit.Key);

                    if(index > lastIndex)
                    {
                        lastIndex = index;
                        lastNumber = digit.Value;
                    }
                }
                sum += firstNumber * 10 + lastNumber;
            }
            Console.WriteLine(sum);
        }
    }
}
