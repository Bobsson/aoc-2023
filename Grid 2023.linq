<Query Kind="Program" />

void Main()
{
	// Unit tests
	var grid = new Grid<int>();
//	grid.Parse(@"000
//000
//000");
}

public class Grid<T> where T : struct
{
	public Grid(bool expandDownRight = true) => ExpandDownRight = expandDownRight;
	public Grid(T def, bool expandDownRight = true) => (DefaultValue, ExpandDownRight) = (def, expandDownRight);

	public T DefaultValue { get; set; }
	public bool ExpandDownRight { get; set; }
	public OutOfBoundsReadBehavior OOBReadBehavior { get; set; } = OutOfBoundsReadBehavior.Default;
	public OutOfBoundsWriteBehavior OOBWriteBehavior { get; set; } = OutOfBoundsWriteBehavior.Expand;
	public int MinX { get; protected set; }
	public int MaxX { get; protected set; }
	public int MinY { get; protected set; }
	public int MaxY { get; protected set; }

	public SortedDictionary<(int x, int y), Point> _grid = new SortedDictionary<(int x, int y), Point>();

	public void Parse(IEnumerable<IEnumerable<T>> i)
	{
		var input = i.Select(c => c.ToArray()).ToArray();
		for (int y = 0; y < input.Length; y++) // rows
		{
			for (int x = 0; x < input[y].Length; x++) // columns
			{
				this[x, y] = input[y][x];
			}
		}
	}

	public T this[(int x, int y) coords]
	{
		get { return this[coords.x, coords.y]; }
		set { this[coords.x, coords.y] = value; }
	}

	public T this[int x, int y]
	{
		get
		{
			if (_grid.ContainsKey((x, y))) return _grid[(x, y)].Value;
			else if (OOBReadBehavior == OutOfBoundsReadBehavior.Error) throw new GridException($"OOB Access");
			else
			{
				var p = new Point(x, y, DefaultValue);
				_grid.Add((x,y), p);
				return p.Value;
			}
		}
		set
		{
			if (x > MaxX || y > MaxY || x < MinX || y < MinY)
			{
				switch (OOBWriteBehavior)
				{
					case OutOfBoundsWriteBehavior.Error: throw new GridException($"OOB Access");
					case OutOfBoundsWriteBehavior.Ignore: return;
					case OutOfBoundsWriteBehavior.Expand:
						{
							if (x < MinX) MinX = x;
							if (x > MaxX) MaxX = x;
							if (y < MinY) MinY = y;
							if (y > MaxY) MaxY = y;
							break;
						}
				}
			}
			if (!_grid.ContainsKey((x, y))) _grid.Add((x, y), new Point(x, y, value));
			_grid[(x, y)] = new Point(x, y, value);
		}
	}
	
	public IEnumerable<Point> GetAllPoints() => _grid.Values;
	public IEnumerable<Point> GetPoints(Func<Point, bool> test) => _grid.Values.Where(test);
	public IEnumerable<(int x, int y)> GetOrthogonal((int x, int y) start)
	{
		yield return (start.x - 1, start.y);
		yield return (start.x + 1, start.y);
		yield return (start.x, start.y - 1);
		yield return (start.x, start.y + 1);
	}		
	public IEnumerable<Point> GetOrthogonal(Point p)
	{
		yield return new Point(p.X, p.Y - 1, this[p.X, p.Y - 1]);
		yield return new Point(p.X, p.Y + 1, this[p.X, p.Y + 1]);
		yield return new Point(p.X - 1, p.Y, this[p.X - 1, p.Y]);
		yield return new Point(p.X + 1, p.Y, this[p.X + 1, p.Y]);
	}
	public IEnumerable<Point> GetAdjacent(Point p) => _grid.Values.Where(v
		=> (v.X == p.X - 1 || v.X == p.X || v.X == p.X + 1)
		&& (v.Y == p.Y - 1 || v.Y == p.Y || v.Y == p.Y + 1)
		&& !(v.X == p.X && v.Y == p.Y)); // Don't want to include itself
	public Point PointAt(int x, int y) => _grid[(x,y)];


	public virtual string Display(bool coords = false)
	{
		StringBuilder sb = new StringBuilder();
		var minCol = ExpandDownRight ? MinY : MaxY * -1;
		var maxCol = ExpandDownRight ? MaxY : MinY * -1;
		var minRow = MinX;// (ZeroUpperLeft ? MaxValue : MinValue);
		var maxRow = MaxX; //(ZeroUpperLeft ? MinValue : MaxValue);

		//$"Displaying grid with rows {minRow}->{maxRow}".Dump();
		if (coords) sb.AppendLine($"({minCol},{minRow}) - ({maxCol},{maxRow})");
		for (int jj = minCol; jj <= maxCol; jj++)
		{
			int j = ExpandDownRight ? jj : jj*-1;
			for (int i = minRow; i <= maxRow; i++)
			{
				if (_grid.ContainsKey((i, j))) sb.Append(_grid[(i, j)].Value);
				else
				{
					//$"Empty row".Dump();
					sb.Append(DefaultValue);
				}
			}
			sb.AppendLine();
		}
		return sb.ToString();
	}

	public record Point
	{
		public int X { get; init; }
		public int Y { get; init; }
		public T Value { get; init; }
		public Point(int x, int y, T value)
		{
			X = x;
			Y = y;
			Value = value;
		}
		public override string ToString() => $"({X}, {Y}) => {Value}";
	}

	public enum OutOfBoundsReadBehavior
	{
		/// <summary>Return the default value, as if the grid was infinite</summary>
		Default,
		/// <summary>Anything that tries to read outside the defined grid will throw an exception</summary>
		Error,
	}
	public enum OutOfBoundsWriteBehavior
	{
		/// <summary>Anything that goes outside the defined grid is silently ignored</summary>
		Ignore,
		/// <summary>Anything that goes outside the defined grid will expand the grid</summary>
		Expand,
		/// <summary>Anything that tries to go outside the defined grid will throw an exception</summary>
		Error
	}
	public class GridException : Exception
	{
		public GridException(string message) : base(message) { }
	}
	public int FloodFillOrthogonal((int x, int y) source, T fill, T? empty = null, T? fillOverflow = null)
	{
		if (empty == null) empty = DefaultValue;
		if (!this[source].Equals(empty.Value)) return 0;
		var queue = new Queue<(int x, int y)>();
		var filled = new List<(int x, int y)>();
		queue.Enqueue(source);
		bool overflow = false;
		while (queue.Count != 0)
		{
			var target = queue.Dequeue();
			foreach (var a in this.GetOrthogonal(target))
			{
				if (a.x > this.MaxX || a.x < this.MinX || a.y > this.MaxY || a.y < this.MinY) { overflow = true; continue;}
				if (this[a].Equals(empty.Value))
				{
					this[a] = fill;
					queue.Enqueue(a);
					filled.Add(a);
				}
			}
		}
		if (overflow)
		{
			if (fillOverflow != null) foreach (var a in filled) { this[a] = fillOverflow.Value; }
			return int.MaxValue;
		}
		else return filled.Count;
	}
}
public class IntGrid : Grid<int>
{
	public IntGrid(int defaultValue) : base(defaultValue, true) { }

	public void Parse(string input) => base.Parse(input.Split("\n").Select(i => i.Trim().ToCharArray().Select(j => j.ToInt())));
}
public class CharGrid : Grid<char>
{
	public CharGrid(char defaultValue) : base(defaultValue, true) { }
	public CharGrid(string input, char defaultValue = '.') : base(defaultValue, true) { Parse(input); }

	public void Parse(string input) => base.Parse(input.Split("\n").Select(i => i.Trim().ToCharArray()));
}
// Not used directly in the grids, but useful to have defined when working with them.
public enum Direction
{
	North,
	NorthEast,
	East,
	SouthEast,
	South,
	SouthWest,
	West,
	NorthWest,
}