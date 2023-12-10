<Query Kind="Statements">
  <DisableMyExtensions>true</DisableMyExtensions>
</Query>

#load "AoC helpers"
#load "!Personal\Advent 2023\Grid 2023"
new Puzzle().Run();


class Puzzle : DailyPuzzle
{
	public override int Day => 10;

	public override string TestInput => @".....
.S-7.
.|.|.
.L-J.
.....";

	public override string TestAnswer1 => "4";
	/*/
        public override string TestInput2 => @".F----7F7F7F7F-7....
    .|F--7||||||||FJ....
    .||.FJ||||||||L7....
    FJL7L7LJLJ||LJ.L-7..
    L--J.L7...LJS7F-7L7.
    ....F-J..F7FJ|L7L7L7
    ....L7.F7||L7|.L7L7|
    .....|FJLJ|FJ|F7|.LJ
    ....FJL-7.||.||||...
    ....L---J.LJ.LJLJ...";

        public override string TestAnswer2 => "8";
    /*/
	public override string TestInput2 => @"FF7FSF7F7F7F7F7F---7
L|LJ||||||||||||F--J
FL-7LJLJ||||||LJL-77
F--JF--7||LJLJ7F7FJ-
L---JF-JLJ.||-FJLJJ7
|F|F-JF---7F7-L7L|7|
|FFJF7L7F-JF7|JL---7
7-L-JL7||F7|L7F-7F7|
L.L7LFJ|||||FJL7||LJ
L7JLJL-JLJLJL--JLJ.L";
	public override string TestAnswer2 => "10";
/**/

	// 0x2550: ═
	// 0x2551: ║
	// 0x2554: ╔
	// 0x2557: ╗
	// 0x255A: ╚
	// 0x255D: ╝

	private void DisplayPipeGrid(CharGrid grid) => grid.Display().Replace('|', '║').Replace('-', '═').Replace('F', '╔').Replace('7', '╗').Replace('L', '╚').Replace('J', '╝').Dump();
	private record Cursor(CharGrid.Point location, Direction sourceDirection, int distance)
	{
		public Cursor GoWest(CharGrid grid) => new Cursor(grid.PointAt(location.X - 1, location.Y), Direction.East, distance + 1);
		public Cursor GoEast(CharGrid grid) => new Cursor(grid.PointAt(location.X + 1, location.Y), Direction.West, distance + 1);
		public Cursor GoNorth(CharGrid grid) => new Cursor(grid.PointAt(location.X, location.Y - 1), Direction.South, distance + 1);
		public Cursor GoSouth(CharGrid grid) => new Cursor(grid.PointAt(location.X, location.Y + 1), Direction.North, distance + 1);
	}

	public override string SolvePart1(string input)
	{
		var grid = new CharGrid(input);
		//DisplayPipeGrid(grid);
		
		var start = grid.GetAllPoints().Single(g => g.Value == 'S');
		var next = FindPath(grid, start).ToArray();
		if (next.Length != 2) throw new Exception("Unclear pipes");
		Cursor cursor1 = next.First(), cursor2 = next.Last();
		
		while (cursor1.location != cursor2.location)
		{
			cursor1 = FindNext(grid, cursor1);
			cursor2 = FindNext(grid, cursor2);
		}
	
		return cursor1.distance.ToString();

	}
	
	private IEnumerable<Cursor> FindPath(CharGrid grid, CharGrid.Point start)
	{
		foreach (var point in grid.GetOrthogonal(start))
		{
			if (point.X == start.X)
			{
				if (point.Y == start.Y + 1 && (point.Value == 'L' || point.Value == '|' || point.Value == 'J'))
					yield return new Cursor(point, Direction.North, 1); // Entered this square from the north
				if (point.Y == start.Y - 1 && (point.Value == '7' || point.Value == '|' || point.Value == 'F'))
					yield return new Cursor(point, Direction.South, 1);
			}
			else if (point.Y == start.Y)
			{
				if (point.X == start.X + 1 && (point.Value == '7' || point.Value == '-' || point.Value == 'J'))
					yield return new Cursor(point, Direction.West, 1);
				if (point.X == start.X - 1 && (point.Value == 'F' || point.Value == '-' || point.Value == 'L'))
					yield return new Cursor(point, Direction.East, 1);
			}
		}
	}
	private Cursor FindNext(CharGrid grid, Cursor current)
	{
		switch (current.location.Value)
		{
			case 'F' when current.sourceDirection == Direction.East:
			case '7' when current.sourceDirection == Direction.West:
			case '|' when current.sourceDirection == Direction.North: return current.GoSouth(grid);
			
			case 'J' when current.sourceDirection == Direction.West:
			case 'L' when current.sourceDirection == Direction.East:
			case '|' when current.sourceDirection == Direction.South: return current.GoNorth(grid);

			case 'J' when current.sourceDirection == Direction.North:
			case '7' when current.sourceDirection == Direction.South:
			case '-' when current.sourceDirection == Direction.East: return current.GoWest(grid);

			case 'L' when current.sourceDirection == Direction.North:
			case 'F' when current.sourceDirection == Direction.South:
			case '-' when current.sourceDirection == Direction.West: return current.GoEast(grid);
		}
		throw new Exception("Unexpected direction");
	}

	public override string SolvePart2(string input)
	{
		var grid = new CharGrid(input);
		//grid.Display().Dump();

		var start = grid.GetAllPoints().Single(g => g.Value == 'S');
		var next = FindPath(grid, start).ToArray();
		if (next.Length != 2) throw new Exception("Unclear pipes");
		Cursor cursor1 = next.First(), cursor2 = next.Last();
		if (cursor1.sourceDirection == Direction.South && cursor2.sourceDirection == Direction.North) grid[start.X, start.Y] = '|';
		if (cursor1.sourceDirection == Direction.East && cursor2.sourceDirection == Direction.West) grid[start.X, start.Y] = '-';
		if (cursor1.sourceDirection == Direction.North && cursor2.sourceDirection == Direction.East) grid[start.X, start.Y] = '7';
		if (cursor1.sourceDirection == Direction.North && cursor2.sourceDirection == Direction.West) grid[start.X, start.Y] = 'F';
		if (cursor1.sourceDirection == Direction.South && cursor2.sourceDirection == Direction.East) grid[start.X, start.Y] = 'J';
		if (cursor1.sourceDirection == Direction.South && cursor2.sourceDirection == Direction.West) grid[start.X, start.Y] = 'L';
		//grid.Display().Dump();
		grid = ZoomIn(grid);
		//grid.Display().Dump();
		grid[start.X * 3, start.Y * 3] = 'S';
		start = grid.PointAt(start.X * 3, start.Y * 3);
		//grid.Display().Dump();
		next = FindPath(grid, start).ToArray();
		if (next.Length != 2) throw new Exception("Unclear pipes");
		cursor1 = next.First();
		cursor2 = next.Last();


		List<Cursor> visited = new List<Puzzle.Cursor>();

		while (cursor1.location != cursor2.location)
		{
			visited.Add(cursor1);
			visited.Add(cursor2);
			cursor1 = FindNext(grid, cursor1);
			cursor2 = FindNext(grid, cursor2);
		}
		visited.Add(cursor1);
		
		// Make the loop we care about stand out.
		foreach (var p in visited)
		{
			switch (grid[p.location.X, p.location.Y])
			{
				case '7': grid[p.location.X, p.location.Y] = '╗'; break;
				case 'J': grid[p.location.X, p.location.Y] = '╝'; break;
				case 'L': grid[p.location.X, p.location.Y] = '╚'; break;
				case 'F': grid[p.location.X, p.location.Y] = '╔'; break;
				case '|': grid[p.location.X, p.location.Y] = '║'; break;
				case '-': grid[p.location.X, p.location.Y] = '═'; break;
			}
		}
		// Duplicate the grid as an easy way to clear all irrelevant pipes
		var clean = new CharGrid(grid.Display().Replace("|",".").Replace("-",".").Replace("F",".").Replace("J",".").Replace("7",".").Replace("L","."));
		char fill = '0';
		foreach (var v in visited)
		{
			var adjacent = clean.GetOrthogonal(v.location);
			bool exit = false;
			foreach (var a in adjacent)
			{
				var filled = clean.FloodFillOrthogonal((a.X, a.Y), fill, '.', '*');
				//filled.Dump("Filled with " + fill);
				if (filled == int.MaxValue) { break; }
				if (filled != 0) fill++;
			}
			if (exit) break;
		}
		//clean.Display().Dump();
		return CountFilled(clean, '0').ToString();
	}
	
	private CharGrid ZoomIn(CharGrid source)
	{
		var result = new CharGrid('.') { OOBWriteBehavior = UserQuery.Grid<char>.OutOfBoundsWriteBehavior.Expand};
		foreach (var point in source.GetAllPoints())
		{
			switch (point.Value)
			{
				case '7':
					result[point.X * 3 - 1, point.Y * 3] = '-';
					result[point.X * 3, point.Y * 3] = '7';
					result[point.X * 3, point.Y * 3 + 1] = '|';
					break;
				case 'J':
					result[point.X * 3 - 1, point.Y * 3] = '-';
					result[point.X * 3, point.Y * 3] = 'J';
					result[point.X * 3, point.Y * 3 - 1] = '|';
					break;
				case 'L':
					result[point.X * 3 + 1, point.Y * 3] = '-';
					result[point.X * 3, point.Y * 3] = 'L';
					result[point.X * 3, point.Y * 3 - 1] = '|';
					break;
				case 'F':
					result[point.X * 3 + 1, point.Y * 3] = '-';
					result[point.X * 3, point.Y * 3] = 'F';
					result[point.X * 3, point.Y * 3 + 1] = '|';
					break;
				case '-':
					result[point.X * 3 - 1, point.Y * 3] = '-';
					result[point.X * 3, point.Y * 3] = '-';
					result[point.X * 3 + 1, point.Y * 3] = '-';
					break;
				case '|':
					result[point.X * 3, point.Y * 3 - 1] = '|';
					result[point.X * 3, point.Y * 3] = '|';
					result[point.X * 3, point.Y * 3 + 1] = '|';
					break;
			}
		}
		return result;
	}
	private int CountFilled(CharGrid source, char fillChar)
	{
		int counter = 0;
		for (int y = 1; y < source.MaxY; y+=3)
		{
			for (int x = 1; x < source.MaxX; x+=3)
			{
				if (source[x-1,y-1] == fillChar
				&&  source[x,y-1] == fillChar
				&& source[x+1,y-1] == fillChar
				&& source[x-1,y] == fillChar
				&& source[x,y] == fillChar
				&& source[x+1,y] == fillChar
				&& source[x-1,y+1] == fillChar
				&& source[x,y+1] == fillChar
				&& source[x+1,y+1] == fillChar)
					counter++;
			}
		}
		return counter;
	}
}