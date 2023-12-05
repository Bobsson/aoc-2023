<Query Kind="Statements">
  <DisableMyExtensions>true</DisableMyExtensions>
</Query>

#load "AoC helpers"
// #load "!Personal\Advent 2023\Grid 2023"
new Puzzle().Run();

/* Version 2, because version 1 tied the parsing code 
 * too closely to the logic and couldn't be expanded for part 2
 */

class Puzzle : DailyPuzzle
{
	const bool dump = false;

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

	public override string TestAnswer2 => "46";

	private static readonly Mapping NoMap = new Mapping(0, 0, 0);
	record Mapping(long sourceStart, long destStart, long size)
	{
		public bool IsApplicable(long target) => target >= sourceStart && target < sourceStart+size;
		public long Map(long target) => (target - sourceStart) + destStart;
		public long sourceEnd => sourceStart + size - 1;
		public long destEnd => destStart + size - 1;
		public long offset => destStart - sourceStart;
	}
	private long Map(IEnumerable<Mapping> maps, long target) => maps.SingleOrDefault(x => x.IsApplicable(target), NoMap).Map(target);
	
	private IEnumerable<Mapping> ParseMappings(string input)
	{
		var lines = input.Split("\n");
		var stage = lines[0];//.Dump();
		foreach (var line in lines.Skip(1))
		{
			var x = line.Split(" ");
			yield return new Mapping(x[1].ToLong(), x[0].ToLong(), x[2].ToLong());
		}
	}

	public override string SolvePart1(string input)
	{
		var parts = input.Replace("\r","").Split("\n\n");
		var seeds = parts[0].Split(" ").Skip(1).Select(long.Parse);//.Dump("Seeds");
		foreach (var p in parts.Skip(1))
		{
			var mappings = ParseMappings(p).ToArray();//.Dump();
			seeds = seeds.Select(s => Map(mappings, s));
		}

		return seeds.Min().ToString();
	}

	public override string SolvePart2(string input)
	{
		var parts = input.Replace("\r", "").Split("\n\n");
		var seeds = parts[0].Split(" ").Skip(1).Select(long.Parse).Chunk(2)
							.Select(s => (first: s[0], last: s[0] + s[1] - 1))
							.ToList();//.Dump("Seeds");
							
		foreach (var p in parts.Skip(1))
		{
			var mappings = ParseMappings(p).ToArray();//.Dump();
			var newValues = new List<(long first, long last)>();
			for (int i = 0; i < seeds.Count; i++)
			{
				var seed = seeds[i];
				bool mapped = false;
				foreach (var map in mappings)
				{
					if (dump) $"Testing: Seed {seed} vs map ({map.sourceStart}, {map.sourceEnd}) (offset={map.offset})".Dump();
					if (seed.first > map.sourceEnd || seed.last < map.sourceStart) continue; // No overlap
					else if (map.sourceStart <= seed.first && seed.last <= map.sourceEnd) // Fully contained
					{
						newValues.Add((seed.first + map.offset, seed.last + map.offset));
						mapped = true;
						if (dump) $"\tNew value: {seed}+{map.offset} -> {newValues.Last()}".Dump();
						break;
					}
					else if (map.sourceStart <= seed.first && map.sourceEnd < seed.last) // Extends beyond
					 {
					 	newValues.Add((seed.first + map.offset, map.sourceEnd + map.offset)); // Overlap 
						seeds.Add((map.sourceEnd + 1, seed.last)); // Leftover, keep for later
						mapped = true;
						if (dump) $"\tNew value: {(seed.first, map.sourceEnd)}+{map.offset} -> {newValues.Last()}".Dump();
						if (dump) $"\tLeftover:  {seeds.Last()}".Dump();
						break;
					 }
					 else if (seed.first < map.sourceStart && seed.last <= map.sourceEnd) // Starts before
					 {
					 	seeds.Add((seed.first, map.sourceStart - 1)); // Leftover, keep for later
						newValues.Add((map.sourceStart + map.offset, seed.last + map.offset)); // Overlap
						mapped = true;
						if (dump) $"\tLeftover:  {seeds.Last()}".Dump();
						if (dump) $"\tNew value: {(map.sourceStart, seed.last)}+{map.offset} -> {newValues.Last()}".Dump();
						break;
					}
					 else if (seed.first < map.sourceStart && map.sourceEnd < seed.last) // Full overlap plus more
					 {
						seeds.Add((seed.first, map.sourceStart - 1)); // Before, keep for later
						if (dump) $"\tLeftover:  {seeds.Last()}".Dump();
						newValues.Add((map.sourceStart + map.offset, map.sourceEnd + map.offset));  // Overlap
						if (dump) $"\tNew value: {(map.sourceStart, map.sourceEnd)}+{map.offset} -> {newValues.Last()}".Dump();
						seeds.Add((map.sourceEnd + 1, seed.last));    // After, keep for later
						if (dump) $"\tLeftover:  {seeds.Last()}".Dump();
						mapped = true;
						break;
					 }
					 else throw new Exception("Missing case");
				}
				if (!mapped) newValues.Add(seed);
			}
			seeds = newValues.ToList();//.Dump();
			//seeds = seeds.Select(s => Map(mappings, s));
		}
		return seeds.Min().first.ToString();
	}
	
	//private (
}