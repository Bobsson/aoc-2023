<Query Kind="Statements">
  <DisableMyExtensions>true</DisableMyExtensions>
</Query>

#load "AoC helpers"
// #load "!Personal\Advent 2023\Grid 2023"
new Puzzle().Run();



class Puzzle : DailyPuzzle
{
	public override int Day => 4;

	public override string TestInput => @"Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11";

	public override string TestAnswer1 => "13";

	//public override string TestInput2 => base.TestInput2;

	public override string TestAnswer2 => "30";

	public override string SolvePart1(string input)
	{
		var cards = input.ReadByLine(i => i.RegexMatch(@"Card +(?'card'\d+): (?: ?(?'winner'\d+) )+\|(?: *(?'played'\d+) ?)+"))
		.Select(i => new { Card = i.Groups["card"].Value, 
							WinningNumbers = i.Groups["winner"].Captures.Select(c => c.Value.ToInt()).Distinct(),
							PlayedNumbers = i.Groups["played"].Captures.Select(c => c.Value.ToInt()).Distinct()
							})
						.Select(i => new { i.Card, i.WinningNumbers, i.PlayedNumbers, Overlap= i.PlayedNumbers.Intersect(i.WinningNumbers)})
		.ToArray();
		//cards.Dump(1);
		//foreach (var c in cards) if (c.WinningNumbers.Count() != 10 || c.PlayedNumbers.Count() != 25) throw new Exception();
		return cards.Select(c => Math.Round(Math.Pow(2, c.Overlap.Count() - 1), 0, MidpointRounding.ToZero)).Dump().Sum().ToString();
	}

	public override string SolvePart2(string input)
	{
		var cards = input.ReadByLine(i => i.RegexMatch(@"Card +(?'card'\d+): (?: ?(?'winner'\d+) )+\|(?: *(?'played'\d+) ?)+"))
						.Select(i => new
						{
							CardId = i.Groups["card"].Value.ToInt(),
							WinningNumbers = i.Groups["winner"].Captures.Select(c => c.Value.ToInt()).Distinct(),
							PlayedNumbers = i.Groups["played"].Captures.Select(c => c.Value.ToInt()).Distinct()
						})
						.Select(i => new { i.CardId, i.WinningNumbers, i.PlayedNumbers, Overlap = i.PlayedNumbers.Intersect(i.WinningNumbers).ToArray() })
						.ToDictionary(i => i.CardId, i => i.Overlap.Length);
		//cards.Dump();
		
		// Start out with one of each card.
		var results = cards.ToDictionary(c => c.Key, c => 1);
		foreach (var card in cards.OrderBy(c => c.Key))
		{
			// Add the current count for this card to the next (winning count) cards
			for (int i = 0; i < card.Value; i++)
			{
				var target = card.Key + i + 1;
				results[target] += results[card.Key];
			}
		}
		//results.Dump();
		return results.Sum(r => r.Value).ToString();
	}
}