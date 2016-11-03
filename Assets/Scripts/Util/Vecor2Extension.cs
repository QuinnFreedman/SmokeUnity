using UnityEngine;
using System.Collections.Generic;

public static class Vector2Extension {

    public static Vector2 Rotate(this Vector2 v, float degrees) {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
        
        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public static Vector2 Towards(this Vector2 self, Vector2 toward) {
		return (toward - self).normalized;
	}

    public static Vector2 Round(this Vector2 self) {
        return new Vector2(Mathf.Round(self.x), Mathf.Round(self.y));
    }

    public static Vector2 Floor(this Vector2 self) {
        return new Vector2(Mathf.Floor(self.x), Mathf.Floor(self.y));
    }

    public static bool AproximatelyEqual(this Vector2 self, Vector2 other, float margin) {
        return Mathf.Abs(self.x - other.x) <= margin && Mathf.Abs(self.y - other.y) <= margin;
    }
}

public static class Vector2Utils {
    
    public static Vector2 Random() {
        var direction = 360 * UnityEngine.Random.value;
        return new Vector2(1, 0).Rotate(direction);
    }

    public static Vector2 Average(List<Vector2> vectors) {
        Vector2 average = Vector2.zero;
        foreach (var vec in vectors) {
            average += vec;
        }
        return average / vectors.Count;
    }
}