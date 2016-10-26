using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(Animator))]
public class CharacterController : MonoBehaviour {

	public float maxMoveSpeed = 5.5f;
	public float acceleration = 1.3f;
	public float friction = 0.65f;
	public float walkToRunSpeedRatio = 0.5f;
	//public float damping = .65f;
	//private Vector2 velocity;
	private Animator anim;
	private Rigidbody2D rb;
	internal bool walking = false;

	// Use this for initialization
	void Start () {
		//velocity = Vector2.zero;
		anim = GetComponent<Animator> ();
		rb = GetComponent<Rigidbody2D> ();
	}

	public void Move(Vector2 direction) {
		if (rb.velocity.x != 0) {
			anim.SetFloat ("LastMoveX", Crush (rb.velocity.x));
			anim.SetFloat ("LastMoveY", 0);
		} else if (rb.velocity.y != 0) {
			anim.SetFloat ("LastMoveY", Crush (rb.velocity.y));
			anim.SetFloat ("LastMoveX", 0);
		}

		if (Mathf.Abs (rb.velocity.x) >= Mathf.Abs (rb.velocity.y)) {
			anim.SetFloat ("MoveX", Crush (rb.velocity.x));
			anim.SetFloat ("MoveY", 0);
		} else {
			anim.SetFloat ("MoveX", 0);
			anim.SetFloat ("MoveY", Crush (rb.velocity.y));
		}

		anim.SetBool ("PlayerMoving", Mathf.Abs(rb.velocity.y) > 0.15 || Mathf.Abs(rb.velocity.x) > 0.15);

		var velocity = new Vector2(rb.velocity.x, rb.velocity.y);

		var force = direction.normalized * acceleration * WalkingFactor;

		var dampX = Mathf.Abs (friction) > Mathf.Abs (velocity.x) ? -velocity.x : -Mathf.Sign (velocity.x) * friction;
		var dampY = Mathf.Abs (friction) > Mathf.Abs (velocity.y) ? -velocity.y : -Mathf.Sign (velocity.y) * friction;

		velocity = new Vector2 (
				Mathf.Clamp (velocity.x + force.x + dampX, -maxMoveSpeed * WalkingFactor, maxMoveSpeed * WalkingFactor),
				Mathf.Clamp (velocity.y + force.y + dampY, -maxMoveSpeed * WalkingFactor, maxMoveSpeed * WalkingFactor));

		rb.velocity = new Vector2 (velocity.x, velocity.y);
	}
	
	private static float Crush(float x) {
		return x < -0.15 ? -1 :
			x > 0.15 ? 1 : 0;
	}

	private float WalkingFactor {
		get {
			return walking ? walkToRunSpeedRatio : 1.0f;
		}
	}

}
