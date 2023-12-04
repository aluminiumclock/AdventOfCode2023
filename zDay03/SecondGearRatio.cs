﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2023
{
    public class SecondGearRatio : IRunnable
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
                    int numbersByGear = 0;
                    int fullNumber = 1;
                    if (lines[i][j] == '*')
                    {
                        if (i > 0)
                        {
                            LineTotal lt = NearbyNumbers(lines[i - 1], j);
                            fullNumber *= lt.totalOnLine;
                            numbersByGear += lt.numbersAdjGearOnLine;
                        }

                        LineTotal clt = NearbyNumbers(lines[i], j);
                        fullNumber *= clt.totalOnLine;
                        numbersByGear += clt.numbersAdjGearOnLine;

                        if (i + 1 < lines.Length)
                        {

                            LineTotal lt = NearbyNumbers(lines[i + 1], j);
                            fullNumber *= lt.totalOnLine;
                            numbersByGear += lt.numbersAdjGearOnLine;
                        }
                    }

                    add = numbersByGear > 1;

                    if (add)
                    {
                        Total += fullNumber;

                    }
                }
            }

            Console.WriteLine(Total);
        }

        private LineTotal NearbyNumbers (string v, int position)
        {
            int total = 1;
            int numbersOnLine = 0;
            int index = position == 0 ? 0 : position - 1;

            if (Char.IsDigit(v[index]))
            {
                int start = index;
                while (start > 0 && Char.IsDigit(v[start - 1]))
                {
                    start--;
                }
                int end = index;
                while (end + 1 < v.Length && Char.IsDigit(v[end + 1]))
                {
                    end++;
                }
                total *= int.Parse(v.Substring(start, end - start + 1));
                numbersOnLine++;
                index = end;
            }

            if(index >= position)
            {
                return new LineTotal
                {
                    totalOnLine = total,
                    numbersAdjGearOnLine = numbersOnLine
                };
            }
            index = position;

            if (Char.IsDigit(v[index]))
            {
                int start = index;
                int end = index;
                while (end + 1 < v.Length && Char.IsDigit(v[end + 1]))
                {
                    end++;
                }
                total *= int.Parse(v.Substring(start, end - start + 1));
                numbersOnLine++;

                index = end;
            }

            if (index >= position+1)
            {
                return new LineTotal
                {
                    totalOnLine = total,
                    numbersAdjGearOnLine = numbersOnLine
                };
            }
            index = position + 1;

            if (Char.IsDigit(v[index]))
            {
                int start = index;
                int end = index;
                while (end + 1 < v.Length && Char.IsDigit(v[end + 1]))
                {
                    end++;
                }
                total *= int.Parse(v.Substring(start, end - start + 1));
                numbersOnLine++;
            }

            return new LineTotal
            {
                totalOnLine = total,
                numbersAdjGearOnLine = numbersOnLine
            };
        }

        private struct LineTotal
        {
            public int totalOnLine;
            public int numbersAdjGearOnLine;
        }

        private bool IsSymbol(char symbol)
        {
            return !(Char.IsLetterOrDigit(symbol) || symbol == '.');
        }
    }
}
