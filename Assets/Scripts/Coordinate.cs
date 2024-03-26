using System;
using UnityEngine;

public class Coordinate
{
	public int X { get; }
	public int Y { get; }
	public int value { get; set; }

	public Coordinate(int x, int y)
	{
		X = x;
		Y = y;
		value = 0;
	}

	// Override Equals and GetHashCode methods to enable coordinate comparison
	public override bool Equals(object obj)
	{
		if (obj == null || GetType() != obj.GetType())
		{
			return false;
		}

		Coordinate other = (Coordinate)obj;
		return X == other.X && Y == other.Y;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(X, Y);
	}

	// Override ToString for debugging or display purposes
	public override string ToString()
	{
		return $"({X}, {Y})";
	}
}
