<Query Kind="Statements">
  <DisableMyExtensions>true</DisableMyExtensions>
</Query>

#load "AoC helpers"
new Puzzle().Run();


class Puzzle : DailyPuzzle
{
	public override int Day => 2;

	public override string TestInput => @"Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green";

	public override string TestAnswer1 => "8";

	//public override string TestInput2 => base.TestInput2;

	public override string TestAnswer2 => "2286";

	private IEnumerable<(int id, Dictionary<string, int> draws)> ParseGames(string input) => 
			input.ReadByLine(i => i.RegexMatch(@"Game (?'game'\d+): (?:(?'round'(?:(?'amount'\d+) (?'color'blue|green|red),? ?)+);? ?)+"))
							.Select(i => (id: i.Groups["game"].Value.ToInt(),
										  draws: i.Groups["amount"].Captures.Zip(i.Groups["color"].Captures, (a, c) => (count: int.Parse(a.Value), color: c.Value))
			 									.GroupBy(c => c.color)
												.ToDictionary(c => c.Key, c => c.Max(n => n.count))
										)
			 				);


	public override string SolvePart1(string input)
	{
		var games = ParseGames(input);
		return games.Where(g => g.draws["green"] <= 13 && g.draws["red"] <= 12 && g.draws["blue"] <= 14)
			.Sum(g => g.id).ToString();
								
	}

	public override string SolvePart2(string input)
	{
		var games = ParseGames(input);
		return games.Select(g => g.draws["red"] * g.draws["blue"] * g.draws["green"])
					.Sum().ToString();
	}
}