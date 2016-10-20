using System;
using UnityEngine;

class Point : IEquatable<Point> {
	public int x, y;
	public Point () {}
	public Point (int x, int y) {
		this.x = x;
		this.y = y;
	}

	public override bool Equals(object other) {
		if (other == null || GetType () != other.GetType ())
			return false;

		var p = (Point)other;
		return p.x == x && p.y == y;
	}

	public bool Equals(Point other) {
		return Equals ((object) other);
	}

	public override int GetHashCode () {
		return x ^ y;
	}

	public static Point FromVector2(Vector2 v) {
		return new Point((int) Mathf.Floor(v.x), (int) Mathf.Floor(v.y));
	}
}

