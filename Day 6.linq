<Query Kind="Statements">
  <DisableMyExtensions>true</DisableMyExtensions>
</Query>

#load "AoC helpers"
// #load "!Personal\Advent 2023\Grid 2023"
new Puzzle().Run();


class Puzzle : DailyPuzzle
{
	public override int Day => 6;

	public override string TestInput => @"Time:      7  15   30
Distance:  9  40  200";

	public override string TestAnswer1 => "288";

	//public override string TestInput2 => base.TestInput2;

	public override string TestAnswer2 => "71503";

	private IEnumerable<(long time, long record)> ParseInput(string input)
	{
		var match = input.Replace("\r", "").RegexMatch(@"^Time: ( *\d+)+\nDistance: ( *\d+)+$", RegexOptions.Multiline);
		return match.Groups[1].Captures.Zip(match.Groups[2].Captures)
					.Select(c => (c.First.Value.ToLong(), c.Second.Value.ToLong())).ToArray();
	}
			

	public override string SolvePart1(string input)
	{
		var races = ParseInput(input);
		long result = 1;
		foreach (var race in races)
		{
			long options = 0;
			for (long t = 0; t <= race.time; t++) // Time holding the button
			{
				if ((race.time - t) * t > race.record) options++;
			}
			result *= options;
		}
		return result.ToString();
	}

	public override string SolvePart2(string input) => 
		SolvePart1(input.Replace(" ", "").Replace(":", ": "));
}