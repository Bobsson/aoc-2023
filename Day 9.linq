<Query Kind="Statements">
  <NuGetReference>morelinq</NuGetReference>
  <Namespace>MoreLinq</Namespace>
  <DisableMyExtensions>true</DisableMyExtensions>
</Query>

#load "AoC helpers"
// #load "!Personal\Advent 2023\Grid 2023"
new Puzzle().Run();


class Puzzle : DailyPuzzle
{
	public override int Day => 9;

	public override string TestInput => @"0 3 6 9 12 15
1 3 6 10 15 21
10 13 16 21 30 45";

	public override string TestAnswer1 => @"114";

	//public override string TestInput2 => base.TestInput2;

	public override string TestAnswer2 => "2";
	
	public override string SolvePart1(string input) =>
		input.ReadByLine(i => i.Split(" ",StringSplitOptions.RemoveEmptyEntries)
								.Select(int.Parse).ToArray())
							.Select(Extrapolate)
							.Sum().ToString();
							
	private int Extrapolate(IEnumerable<int> values)
	{
		//$"Extrapolating from {values.Join(",")}".Dump();
		if (values.All(v => v == 0)) return 0;
		
		var differences = values.Window(2).Select(v => v[1]-v[0]).ToList();
		var next = Extrapolate(differences) + values.Last();               
		//$"\tNext value: {next}".Dump();
		return next;
	}

	public override string SolvePart2(string input) =>
		input.ReadByLine(i => i.Split(" ", StringSplitOptions.RemoveEmptyEntries)
						.Select(int.Parse).Reverse().ToArray())
					.Select(Extrapolate)
					.Sum().ToString();
}