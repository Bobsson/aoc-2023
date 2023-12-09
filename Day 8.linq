<Query Kind="Statements">
  <DisableMyExtensions>true</DisableMyExtensions>
</Query>

#load "AoC helpers"
// #load "!Personal\Advent 2023\Grid 2023"
new Puzzle().Run();


class Puzzle : DailyPuzzle
{
	public override int Day => 8;

/**/
	public override string TestInput => @"RL

AAA = (BBB, CCC)
BBB = (DDD, EEE)
CCC = (ZZZ, GGG)
DDD = (DDD, DDD)
EEE = (EEE, EEE)
GGG = (GGG, GGG)
ZZZ = (ZZZ, ZZZ)";

	public override string TestAnswer1 => "2";
	/*/
		public override string TestInput => @"LLR

AAA = (BBB, BBB)
BBB = (AAA, ZZZ)
ZZZ = (ZZZ, ZZZ)";

		public override string TestAnswer1 => "6";
	/**/

	public override string TestInput2 => @"LR

11A = (11B, XXX)
11B = (XXX, 11Z)
11Z = (11B, XXX)
22A = (22B, XXX)
22B = (22C, 22C)
22C = (22Z, 22Z)
22Z = (22B, 22B)
XXX = (XXX, XXX)";

	public override string TestAnswer2 => null;

	private record Node(string name, string left, string right);

	private (string directions, Dictionary<string, Node> nodes) ParseMap(string input)
	{
		var match = input.RegexMatch(@"^(?'directions'[RL]+)\s+(?:(?'start'\w{3}) = \((?'left'\w{3}), (?'right'\w{3})\)\s*)+$");
		return (match.Groups["directions"].Value, 
				match.Groups["start"].Captures.Zip(match.Groups["left"].Captures, match.Groups["right"].Captures)
					 .Select(c => new Node(c.First.Value,c.Second.Value,c.Third.Value))
					 .ToDictionary(c => c.name, c => c));
	}
			

	public override string SolvePart1(string input)
	{
		var map = ParseMap(input);//.Dump();
		map.nodes.Count(n => n.Key[2] == 'A').Dump("A's");
		map.nodes.Count(n => n.Key[2] == 'Z').Dump("Z's");
		map.directions.Length.Dump("Length");
		var current = "AAA";
		int step = -1;
		long distance = 0;
		while (current != "ZZZ")
		{
			if (++step >= map.directions.Length) step = 0; // Wrap around
			
			if (map.directions[step] == 'R') current = map.nodes[current].right;
			if (map.directions[step] == 'L') current = map.nodes[current].left;
			distance++;
		}
		
		return distance.ToString();
	}

	private record Route(string startNode, int startStep, long distance, string end);

	public override string SolvePart2(string input)
	{
		throw new NotImplementedException();
	}
}