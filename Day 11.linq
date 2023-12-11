<Query Kind="Statements">
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

	public override string TestAnswer2 => null;

	public override string SolvePart1(string input)
	{
		var grid = new CharGrid(input);
	}

	public override string SolvePart2(string input)
	{
		throw new NotImplementedException();
	}
}