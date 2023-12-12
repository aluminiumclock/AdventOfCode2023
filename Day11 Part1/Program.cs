using System.Collections.Generic;

string inputFile;

#if DEBUG
    inputFile = "test.txt";
#else
    inputFile = "input.txt";
#endif

int result = 0;
string[] input = File.ReadAllLines(inputFile);

int width = input[0].Length;
int height = input.Length;
int GalaxyNo = 1;
List<StarChartPoint> StarChart = new();

for (int x = 0 ; x < width; x++)
{
    for(int y = 0 ; y < height; y++)
    {
        if (input[x][y] == '#')
        {
            StarChart.Add(new StarChartPoint(x, y, GalaxyNo, true));
            GalaxyNo++;
        }
        else
        {
            StarChart.Add(new StarChartPoint(x, y, -1, false));
        }
    }
}

List<int> EmptyRows = StarChart.GroupBy(x => new { x.x, x.isGalaxy })
                        .GroupBy(z => z.Key.x)
                        .Select(grp => new { row = grp.Key, num = grp.Count()})
                        .Where(p => p.num == 1).
                        Select (row => row.row).ToList();

List<int> EmptyColumns = StarChart.GroupBy(x => new { x.y, x.isGalaxy })
                        .GroupBy(z => z.Key.y)
                        .Select(grp => new { column = grp.Key, num = grp.Count() })
                        .Where(p => p.num == 1).
                        Select(q => q.column).ToList();

var Galaxies = StarChart.Where(x => x.isGalaxy);


foreach(var galaxy in Galaxies)
{
    int iD = galaxy.galaxyNo;
    foreach(var nextGalaxy in Galaxies.Where(x => x.galaxyNo > iD))
    {
        int emptyRows = EmptyRows.Where(x => x > galaxy.x && x < nextGalaxy.x).Count();
        int emptyColumns = EmptyColumns.Where(y => y > Math.Min(galaxy.y, nextGalaxy.y) && y < Math.Max(galaxy.y, nextGalaxy.y)).Count();

        int rowDiffernce = nextGalaxy.x - galaxy.x;
        int columnDifference = Math.Max(galaxy.y, nextGalaxy.y) - Math.Min(galaxy.y, nextGalaxy.y);
        int total = rowDiffernce + columnDifference + emptyRows + emptyColumns;
        result += total;
    }
}

Console.WriteLine(result);
Console.ReadLine();

class StarChartPoint
{
    public int x;
    public int y;
    public int galaxyNo;
    public bool isGalaxy;

    public StarChartPoint(int x, int y, int galaxyNo, bool isGalaxy)
    {
        this.x = x;
        this.y = y;
        this.galaxyNo = galaxyNo;
        this.isGalaxy = isGalaxy;
    }
}