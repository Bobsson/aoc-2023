<Query Kind="Statements">
  <DisableMyExtensions>true</DisableMyExtensions>
</Query>

#load "AoC helpers"
#load "!Personal\Advent 2023\Grid 2023"

new Puzzle().Run();


class Puzzle : DailyPuzzle
{
	public override int Day => 3;

	public override string TestInput => @"467..114..
...*......
..35..633.
......#...
617*......
.....+.58.
..592.....
......755.
...$.*....
.664.598..";

	public override string TestAnswer1 => "4361";

	//public override string TestInput2 => base.TestInput2;

	public override string TestAnswer2 => "467835";

	public override string SolvePart1(string input)
	{
		var grid = new CharGrid(input);
		//grid.Display().Dump();
		
		// Find the start to each number
		var nums = grid.GetPoints(g => char.IsDigit(g.Value) && !char.IsDigit(grid[g.X-1, g.Y]))
			// then find the rest of it.
			.Select(g => grid.GetPoints(gg => gg.X >= g.X && gg.Y == g.Y)
							 .TakeWhile(gg => char.IsDigit(gg.Value)))
		.ToList();
		
		//nums.Dump();
		
		// Check for a symbol next to any of the digits of each number.
		var total = 0;
		foreach (var n in nums)
		{
			bool end = false;
			foreach (var d in n)
			{
				foreach (var adj in grid.GetAdjacent(d))
				{
					if (adj.Value != '.' && !char.IsDigit(adj.Value))
					{
						total += n.Select(gg => gg.Value).Join("").ToInt();
						end = true;
						break;
					}
				}
				if (end) break;
			}
		}
		return total.ToString();
	}

	public override string SolvePart2(string input)
	{
		var grid = new CharGrid(input);
		//grid.Display().Dump();

		// Find the start to each number
		var nums = grid.GetPoints(g => char.IsDigit(g.Value) && !char.IsDigit(grid[g.X - 1, g.Y]))
			// then find the rest of it.
			.Select(g => grid.GetPoints(gg => gg.X >= g.X && gg.Y == g.Y)
							 .TakeWhile(gg => char.IsDigit(gg.Value)))
		.ToList();

		//nums.Dump();

		var potentialGearValues = new Dictionary<(int x, int y), long>();
		var realGearValues = new Dictionary<(int x, int y), long>();
		foreach (var n in nums)
		{
			bool end = false;
			foreach (var d in n)
			{
				foreach (var adj in grid.GetAdjacent(d))
				{
					var p = (adj.X, adj.Y);
					if (adj.Value == '*')
					{
						if (realGearValues.ContainsKey(p)) throw new Exception("Triple gear");
						else if (potentialGearValues.ContainsKey(p))
						{
							// Next to a real gear, count it.
							realGearValues[p] = potentialGearValues[p] * n.Select(gg => gg.Value).Join("").ToInt();
							potentialGearValues.Remove(p);
						}
						else
						{
							// Next to a potential gear, log it.
							potentialGearValues[p] = n.Select(gg => gg.Value).Join("").ToInt();
						}
						end = true;
						break;
					}
				}
				if (end) break;
			}
		}
		return realGearValues.Select(gv => gv.Value).Sum().ToString();

	}
}