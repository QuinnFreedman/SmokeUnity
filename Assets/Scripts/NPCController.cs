using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class NPCController : MonoBehaviour {

	private Transform target;
	private Mode mode = Mode.FOLLOW;
	private CharacterController controller;
	public float maxFollowDistance = 1f;
	public float minFollowDistance = 0.8f;

	enum Mode {
		FOLLOW, SEARCH
	}

	void Start () {
		target = GameObject.Find("player").transform;
		controller = GetComponent<CharacterController>();
	}
	
	void FixedUpdate () {
		switch (mode) {
			case Mode.FOLLOW:
				Vector3 direction = (target.transform.position - transform.position).normalized;
				float distance = Vector2.Distance(target.transform.position, transform.position);
				if (distance <= maxFollowDistance) {
					if (distance < minFollowDistance) {
						controller.Move(-direction);
					} else {
						controller.Move(Vector2.zero);
					}
				} else {
					RaycastHit2D hitinfo = Physics2D.Raycast(transform.position, direction, distance, 1 << 8);
					print(hitinfo.collider);
					if (hitinfo) {
						Debug.DrawRay(transform.position, direction, Color.red, 0.5f);
					} else {
						Debug.DrawRay(transform.position, direction, Color.green, 0.5f);
						controller.Move(direction);
					}
				}
			break;
		}
	}

	void OnDrawGizmos() {
		switch (mode) {
			case Mode.FOLLOW:
				Gizmos.color = Color.red;
			break;
			case Mode.SEARCH:
				Gizmos.color = Color.green;
			break;
		}
		Gizmos.DrawSphere(GetComponent<Transform>().position + Vector3.up * 0.7f, 0.2f);
	}
}
