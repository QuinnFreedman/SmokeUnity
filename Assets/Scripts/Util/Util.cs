using UnityEngine;

public class Util {
    public static void DrawStar(Vector2 origin, float radius=0.5f, Color color=default(Color), float durration=0) {
        Vector2 vec = Vector2.up * radius;
        for (int i = 0; i < 8; i++) {
            Debug.DrawLine(origin, origin + vec, color, durration);
            vec = vec.Rotate(45f);
        }
    }

	public static float Crush(float x) {
		return x < -0.15 ? -1 :
			x > 0.15 ? 1 : 0;
	}
}