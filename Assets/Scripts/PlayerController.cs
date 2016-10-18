using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {

	public float maxMoveSpeed = 5.5f;
	public float acceleration = 1.3f;
	public float friction = 0.65f;
	//public float damping = .65f;
	//private Vector2 velocity;
	private Animator anim;
	private Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		//velocity = Vector2.zero;
		anim = GetComponent<Animator> ();
		rb = GetComponent<Rigidbody2D> ();
	}

	void FixedUpdate() {
		
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

		var input = new Vector2(Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical")).normalized * acceleration;

		var dampX = Mathf.Abs (friction) > Mathf.Abs (velocity.x) ? -velocity.x : -Mathf.Sign (velocity.x) * friction;
		var dampY = Mathf.Abs (friction) > Mathf.Abs (velocity.y) ? -velocity.y : -Mathf.Sign (velocity.y) * friction;

		velocity = new Vector2 (
				Mathf.Clamp (velocity.x + input.x + dampX, -maxMoveSpeed, maxMoveSpeed),
				Mathf.Clamp (velocity.y + input.y + dampY, -maxMoveSpeed, maxMoveSpeed));
		//velocity.Normalize();

		rb.velocity = new Vector2 (velocity.x, velocity.y);
	}
	
	// Update is called once per frame
	/*void Update () {

		if (velocity.x != 0) {
			anim.SetFloat ("LastMoveX", Crush (velocity.x));
			anim.SetFloat ("LastMoveY", 0);
		} else if (velocity.y != 0) {
			anim.SetFloat ("LastMoveY", Crush (velocity.y));
			anim.SetFloat ("LastMoveX", 0);
		}

		if (Input.GetAxisRaw ("Horizontal") > 0) {
			velocity.x += acceleration;
		} else if (Input.GetAxisRaw ("Horizontal") < 0) {
			velocity.x -= acceleration;
		} else {
			float dx = damping * -Mathf.Sign (velocity.x);
			velocity.x = Mathf.Abs (dx) >= Mathf.Abs (velocity.x) ? 0 : velocity.x + dx;
		}

		if (Input.GetAxisRaw ("Vertical") > 0) {
			velocity.y += acceleration;
		} else if (Input.GetAxisRaw ("Vertical") < 0) {
			velocity.y -= acceleration;
		} else {
			float dy = damping * -Mathf.Sign (velocity.y);
			velocity.y = Mathf.Abs (dy) >= Mathf.Abs (velocity.y) ? 0 : velocity.y + dy;
		}

		velocity.x = Mathf.Clamp (velocity.x, -maxMoveSpeed, maxMoveSpeed);
		velocity.y = Mathf.Clamp (velocity.y, -maxMoveSpeed, maxMoveSpeed);

		transform.Translate (velocity * Time.deltaTime);

		anim.SetFloat ("MoveX", Crush (velocity.x));
		anim.SetFloat ("MoveY", Crush (velocity.y));
		anim.SetBool ("PlayerMoving", velocity.y != 0 || velocity.x != 0);
	}*/

	private float Crush(float x) {
		return x < -0.15 ? -1 :
			x > 0.15 ? 1 : 0;
	}

}
