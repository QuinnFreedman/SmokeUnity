using System;

class Point : IEquatable<Point> {
	public int x, y;
	public Point () {}
	public Point (int x, int y) {
		this.x = x;
		this.y = y;
	}

	public override bool Equals(Object other) {
		if (other == null || GetType () != other.GetType ())
			return false;

		var p = (Point)other;
		return p.x == x && p.y == y;
	}

	public bool Equals(Point other) {
		return Equals ((Object) other);
	}

	public override int GetHashCode () {
		return x ^ y;
	}
}

