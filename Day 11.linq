<Query Kind="Statements">
  <NuGetReference>morelinq</NuGetReference>
  <Namespace>MoreLinq</Namespace>
  <DisableMyExtensions>true</DisableMyExtensions>
</Query>

#load "AoC helpers"
#load "!Personal\Advent 2023\Grid 2023"
new Puzzle().Run();


class Puzzle : DailyPuzzle
{
	public override int Day => 11;

	public override string TestInput => @"...#......
.......#..
#.........
..........
......#...
.#........
.........#
..........
.......#..
#...#.....";

	public override string TestAnswer1 => "374";

	//public override string TestInput2 => base.TestInput2;

	public override string TestAnswer2 => "82000210";

	public override string SolvePart1(string input)
	{
		var lines = input.Split("\n", StringSplitOptions.TrimEntries).ToList();
		var width = lines[0].Length;
		var blankLine = new string('.', width);
		// Perform space expansion
		for (int i = 0; i < lines.Count; i++)
		{
			if (lines[i] == blankLine) lines.Insert(i++, blankLine);
		}
		for (int i = 0; i < width; i++)
		{
			if (lines.All(l => l[i] == '.' || l[i] == '-'))
			{
				for (int l = 0; l < lines.Count; l++)
				{
					lines[l] = lines[l].Remove(i, 1).Insert(i, "..");
				}
				i++;
				width++;
			}
		}
		
		// Find galaxies
		var galaxies = new List<(int x, int y)>();
		for (int y = 0; y < lines.Count; y++)
		{
			for (int x = 0; x < width; x++)
			{
				if(lines[y][x] == '#') galaxies.Add((x, y));
			}
		}
		var paths = galaxies.Cartesian(galaxies, (a, b) => Distance(a, b));
		
		return (paths.Sum() / 2).ToString();
	}
	private long Distance((long x, long y) a, (long x, long y) b) => Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);

	public override string SolvePart2(string input)
	{
		var lines = input.Split("\n", StringSplitOptions.TrimEntries).ToList();
		var width = lines[0].Length;
		var blankLine = new string('.', width);
		// Perform space expansion
		for (int i = 0; i < lines.Count; i++)
		{
			if (lines[i] == blankLine) lines[i] = new string('-', width);
		}
		for (int i = 0; i < width; i++)
		{
			if (lines.All(l => l[i] == '.' || l[i] == '-'))
			{
				for (int l = 0; l < lines.Count; l++)
				{
					if (lines[l][i] == '-') 
						lines[l] = lines[l].Remove(i, 1).Insert(i, "+");
					else
						lines[l] = lines[l].Remove(i, 1).Insert(i, "|");
				}
				i++;
			}
		}
		lines.Join("\n").Dump();

		// Find galaxies
		var galaxies = new List<(long x, long y)>();
		long expansion = 1_000_000-1;
		long extraY = 0;
		for (int y = 0; y < lines.Count; y++)
		{
			if (lines[y].Contains("-")) extraY += expansion;
			else
			{
				long extraX = 0;
				for (int x = 0; x < width; x++)
				{
					if (lines[y][x] == '|') extraX += expansion;
					if (lines[y][x] == '#') galaxies.Add((x + extraX, y + extraY));
				}
			}
		}
		var paths = galaxies.Cartesian(galaxies, (a, b) => Distance(a, b));

		return (paths.Sum() / 2).ToString();
	}
}