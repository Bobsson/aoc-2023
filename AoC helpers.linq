<Query Kind="Program">
  <Namespace>System.Net</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

public abstract class DailyPuzzle
{
	public abstract int Day { get; }
	public abstract string TestInput { get; }
	public abstract string TestAnswer1 { get; }
	public virtual string TestInput2 => TestInput;
	public abstract string TestAnswer2 { get; }
	public string RealInput => ReadAdventInput(Day).GetAwaiter().GetResult();
	public abstract string SolvePart1(string input);
	public abstract string SolvePart2(string input);
	public bool Test() => SolvePart1(TestInput).Dump("Test 1") == TestAnswer1 && (TestAnswer2 == null || SolvePart2(TestInput2).Dump("Test 2") == TestAnswer2);
	public void Solve() { SolvePart1(RealInput).Dump("Part 1"); if (TestAnswer2 != null) SolvePart2(RealInput).Dump("Part 2"); }
	
	public void Run()
	{
		if (Test()) Solve();
		else { "INVALID".Dump(); TestAnswer1.Dump("Expected 1"); TestAnswer2.Dump("Expected 2"); }
	}
	
	public override string ToString() => $"Day {Day}: Part1 test = {SolvePart1(TestInput)}; real = {SolvePart1(RealInput)}\n" + (TestAnswer2 != null ? "Part 2 test = {SolvePart2(TestInput)}; real = {SolvePart2(RealInput)}." : "Part 2 missing");
}


void OnStart()
{
	Util.RawHtml("<style> body, pre { font-family:consolas } </style>").Dump();
}
public static async Task<string> ReadAdventInput(int day)
{
	if (Util.CurrentQueryPath == null) throw new Exception("Query must be saved first");
	var filename = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), $"Input {day}.txt");
	if (File.Exists(filename)) return File.ReadAllText(filename);
	else
	{
		using var web = new HttpClient();
		web.DefaultRequestHeaders.Add("Cookie", "session=" + Util.GetPassword("Advent 2023 Session"));
		var data = await web.GetStringAsync(@"https://adventofcode.com/2023/day/" + day + "/input");
		//var data = web.DownloadString(@"https://adventofcode.com/2021/day/" + day + "/input");
		data = data.Replace("\n", "\r\n");//.Trim();
		data = data.TrimEnd();
		File.WriteAllText(filename, data);
		return data;
	}
}
public static class AdventHelpers
{
	public static IEnumerable<T> ReadByLine<T>(this string input, Func<string, T> operation)
	{
		foreach (var line in input.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
		{
			if (string.IsNullOrWhiteSpace(line)) continue;
			yield return operation(line.Trim());
		}
	}
	public static int ToInt(this char c) => c switch
	{
		>= '0' and <= '9' => c - '0',
		_ => throw new ArgumentException("Non-digit character: '" + c + "'", nameof(c))
	};

	// Making some frequently used operations fluent.
	public static int ToInt(this string s) => int.Parse(s);
	public static Match RegexMatch(this string input, string pattern) => Regex.Match(input, pattern);
	public static Match RegexMatch(this string input, string pattern, RegexOptions options) => 	Regex.Match(input, pattern, options);
	public static string Join<T>(this IEnumerable<T> list, string joiner) => string.Join(joiner, list);
}

public class DataReader
{
	private string _data;
	public int position { get; private set; } = 0;
	public DataReader(string data) => _data = data;
	public string Read(int chars)
	{
		if (position >= _data.Length) return null;
		if (position + chars > _data.Length) return _data.Substring(position);
		var ret = _data.Substring(position, chars);
		position += chars;
		return ret;
	}
	public bool HasMoreData => position < _data.Length;

}
