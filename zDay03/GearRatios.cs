using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    public class GearRatios : IRunnable
    {
        public void Run()
        {
            int Total = 0;

            string[] lines = File.ReadAllLines(@"Day03\Input.txt");
            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
                {
                    bool add = false;
                    int start = j;
                    int fullNumber = 0;
                    if (Char.IsDigit(lines[i][j]))
                    {
                        while(j + 1 < lines[i].Length && Char.IsDigit(lines[i][j + 1]))
                        {
                            j++;
                        }
                        fullNumber = int.Parse(lines[i].Substring(start, j - start + 1));

                        if (i > 0)
                        {
                            if(CheckLineForSymbol(lines[i - 1], start, j)) { add = true; }
                        }

                        if (CheckLineForSymbol(lines[i], start, j)) { add = true; }

                        if(i + 1 < lines.Length)
                        {
                            if (CheckLineForSymbol(lines[i + 1], start, j)) { add = true; }
                        }
                    }

                    if (add)
                    {
                        Total += fullNumber;

                    }
                }
            }

            Console.WriteLine(Total);
        }

        private bool CheckLineForSymbol(string v, int start, int end)
        {
            start = start != 0 ? start-1 : start;
            end = end + 1 == v.Length ? end : end + 1;
            for (int i = start; i <= end; i++)
            {
                if (IsSymbol(v[i])) { return true; }
            }
            return false;
        }

        private bool IsSymbol(char symbol)
        {
            return !(Char.IsLetterOrDigit(symbol) || symbol == '.');
        }
    }
}
