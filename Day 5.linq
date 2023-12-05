<Query Kind="Statements">
  <DisableMyExtensions>true</DisableMyExtensions>
</Query>

#load "AoC helpers"
// #load "!Personal\Advent 2023\Grid 2023"
new Puzzle().Run();


class Puzzle : DailyPuzzle
{
	public override int Day => 5;

	public override string TestInput => @"seeds: 79 14 55 13

seed-to-soil map:
50 98 2
52 50 48

soil-to-fertilizer map:
0 15 37
37 52 2
39 0 15

fertilizer-to-water map:
49 53 8
0 11 42
42 0 7
57 7 4

water-to-light map:
88 18 7
18 25 70

light-to-temperature map:
45 77 23
81 45 19
68 64 13

temperature-to-humidity map:
0 69 1
1 0 69

humidity-to-location map:
60 56 37
56 93 4";

	public override string TestAnswer1 => "35";

	//public override string TestInput2 => base.TestInput2;

	public override string TestAnswer2 => null;

	public override string SolvePart1(string input)
	{
		List<long> targetNums = null;
		var reader = new StringReader(input);
		string line = reader.ReadLine();
		while (line != null)
		{
			if (line.StartsWith("seeds:"))
			{
				targetNums = line.Split(" ").Skip(1).Select(long.Parse).ToList();
				targetNums.Dump("Seeds");
			}
			else
			{
				var state = reader.ReadLine();
				targetNums = PerformMapping(reader, targetNums).ToList();
				targetNums.Dump(state);
			}
			line = reader.ReadLine();
		}
		return null;
	}
	
	private IEnumerable<long> PerformMapping(StringReader input, List<long> targets)
	{
		var line = input.ReadLine();
		while (line != "")
		{
			var values = line.Split(" ").Select(long.Parse).ToArray();
			foreach (var target in targets)
			{
				// This line affects this target
				if (target > values[0] && target < values[0] + values[2])
				{
					yield return GetOffset(target, values[0], values[1]);
					targets.Remove(target);
					break;
				}
			}
			line = input.ReadLine();
		}
		// Anything still here didn't get mapped, so just return it as-is
		foreach (var target in targets)
		{
			yield return target;
		}
	}
	
	private long GetOffset(long target, long startA, long startB) => (target - startA) + startB;

	public override string SolvePart2(string input)
	{
		throw new NotImplementedException();
	}
}