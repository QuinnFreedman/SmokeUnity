using UnityEngine;	

public class CameraController : MonoBehaviour {

	public Transform followTarget;
	public float acceleration = 50f;
	public float damping = 0.8f;
	private Vector3 previousPosition;
	private Vector3 currentPosition;

	private float xVelocity = 0f;
	private float yVelocity = 0f;

	// Use this for initialization
	void Start () {
		currentPosition = followTarget.position;
		previousPosition = currentPosition;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		previousPosition = currentPosition;
		currentPosition = followTarget.position;

		float targetX = Mathf.LerpUnclamped (previousPosition.x, currentPosition.x, acceleration);
		float targetY = Mathf.LerpUnclamped (previousPosition.y, currentPosition.y, acceleration);

		float x = Mathf.SmoothDamp (transform.position.x, targetX, ref yVelocity, damping);
		float y = Mathf.SmoothDamp (transform.position.y, targetY, ref xVelocity, damping);

		transform.position = new Vector3 (x, y, transform.position.z);
	}
}
