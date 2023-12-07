<Query Kind="Statements">
  <DisableMyExtensions>true</DisableMyExtensions>
</Query>

#load "AoC helpers"
// #load "!Personal\Advent 2023\Grid 2023"
new Puzzle().Run();


class Puzzle : DailyPuzzle
{
	public override int Day => 7;

	public override string TestInput => @"32T3K 765
T55J5 684
KK677 28
KTJJT 220
QQQJA 483";

	public override string TestAnswer1 => "6440";

	//public override string TestInput2 => base.TestInput2;

	public override string TestAnswer2 => "5905";

	enum HandType
	{
		HighCard,
		OnePair,
		TwoPair,
		ThreeOfAKind,
		FullHouse,
		FourOfAKind,
		FiveOfAKind,
	}
	private class Hand : IComparable<Hand>, IComparer<char>
	{
		public char[] Cards { get; }
		public HandType Type { get; }
		public bool UsingJokers { get; }
		//public char HighCard => Cards[0];

		public Hand(string input, bool jokers = false)
		{
			Cards = input.ToArray();
			var hand = input.GroupBy(i => i).OrderByDescending(i => i.Count()).ToArray();
			if (jokers) 
			{
				// The best thing to do with a joker is make it the same as whatever else you have the most of...
				// ... unless all you have is jokers - then they all become aces.
				// So apply that rule, and then re-sort.
				hand = input.Replace('J', hand.FirstOrDefault(h => h.Key != 'J', null)?.Key ?? 'A').GroupBy(i => i).OrderByDescending(i => i.Count()).ToArray();
				UsingJokers = true;
			}
			if (hand.Count() == 1) Type = HandType.FiveOfAKind; // All the same
			else if (hand[0].Count() == 4) Type = HandType.FourOfAKind;
			else if (hand[0].Count() == 3 && hand[1].Count() == 2) Type = HandType.FullHouse;
			else if (hand[0].Count() == 3) Type = HandType.ThreeOfAKind;
			else if (hand[0].Count() == 2 && hand[1].Count() == 2) Type = HandType.TwoPair;
			else if (hand[0].Count() == 2) Type = HandType.OnePair;
			else Type = HandType.HighCard;
		}
		public int CompareTo(Hand other)
		{
			if (this.Cards == other.Cards) return 0;
			var result = this.Type.CompareTo(other.Type);
			if (result != 0) return result;
			for (int i = 0; i < 5; i++)
			{
				result = Compare(this.Cards[i], other.Cards[i]);
				if (result != 0) return result;
			}
			return 0;
		}

		/*  A, K, Q, J, T, 9, 8, 7, 6, 5, 4, 3, or 2. The relative strength of each card 
 		 * follows this order, where A is the highest and 2 is the lowest.*/
		public int Compare(char x, char y)
		{
			int left, right;
			if (x == 'A') left = 14;
			else if (x == 'K') left = 13;
			else if (x == 'Q') left = 12;
			else if (x == 'J') left = (UsingJokers ? 1 : 11);
			else if (x == 'T') left = 10;
			else left = x - '0';
			if (y == 'A') right = 14;
			else if (y == 'K') right = 13;
			else if (y == 'Q') right = 12;
			else if (y == 'J') right = (UsingJokers ? 1 : 11);
			else if (y == 'T') right = 10;
			else right = y - '0';

			return left.CompareTo(right);
		}

		public override string ToString() => new string(Cards) + " => " + Type;

	}

	public override string SolvePart1(string input)
	{
		var hands = input.ReadByLine(i => i.Split(" ")).Select(h => (hand: new Hand(h[0]),
														bid: h[1].ToLong())).ToList();
		return hands.OrderBy(x => x.hand).Select((x,i) => x.bid * (i+1)).Sum().ToString();
	}

	public override string SolvePart2(string input)
	{
		var hands = input.ReadByLine(i => i.Split(" ")).Select(h => (hand: new Hand(h[0], true),
														bid: h[1].ToLong())).ToList();
		return hands.OrderBy(x => x.hand).Select((x, i) => x.bid * (i + 1)).Sum().ToString();
	}
}