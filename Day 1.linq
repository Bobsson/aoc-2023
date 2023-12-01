<Query Kind="Statements">
  <DisableMyExtensions>true</DisableMyExtensions>
</Query>

#load "AoC helpers"
new Puzzle().Run();



class Puzzle : DailyPuzzle
{
	public override int Day => 1;

	public override string TestInput => @"1abc2
pqr3stu8vwx
a1b2c3d4e5f
treb7uchet";

	public override string TestInput2 => @"two1nine
eightwothree
abcone2threexyz
xtwone3four
4nineeightseven2
zoneight234
7pqrstsixteen";

	public override string TestAnswer1 => @"142";

	// 29, 83, 13, 24, 42, 14, and 76
	public override string TestAnswer2 => @"281";

	/* On each line, the calibration value can be found by combining the first digit and the last digit (in that order) to form a single two-digit number. */
	public override string SolvePart1(string input) => 
		input.ReadByLine(i => i.RegexMatch(@"^.*?(\d)").Groups[1].Value + i.RegexMatch(@".*(\d).*?$").Groups[1].Value)//.Dump()
			.Select(int.Parse).Sum().ToString();


	public override string SolvePart2(string input) => 
		input.ReadByLine(i => new Regex(@"^.*?(\d|one|two|three|four|five|six|seven|eight|nine)").Match(i).Groups[1].Value
						    + new Regex(@".*(\d|one|two|three|four|five|six|seven|eight|nine).*?$").Match(i).Groups[1].Value)//.Dump()
			.Select(i => i.Replace("one", "1")
							 .Replace("two", "2")
							 .Replace("three", "3")
							 .Replace("four", "4")
							 .Replace("five", "5")
							 .Replace("six", "6")
							 .Replace("seven", "7")
							 .Replace("eight", "8")
							 .Replace("nine", "9"))
			.Select(int.Parse).Sum().ToString();
}