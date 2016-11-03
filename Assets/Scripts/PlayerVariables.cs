using UnityEngine;


public class PlayerVariables : MonoBehaviour {

	private float worldX;
	private float worldY;
	public int maxHealth;
	public int health;

	void Update () {
		worldX = transform.position.x;
		worldY = transform.position.y;
	}
}
