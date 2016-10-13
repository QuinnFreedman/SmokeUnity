using UnityEngine;	

public class CameraController : MonoBehaviour {

	public Transform camera;
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
	void Update () {
		previousPosition = currentPosition;
		currentPosition = followTarget.position;

		float targetX = Mathf.LerpUnclamped (previousPosition.x, currentPosition.x, acceleration);
		float targetY = Mathf.LerpUnclamped (previousPosition.y, currentPosition.y, acceleration);

		//Vector3 extrapolatedTarget = 

		//float distance = ((Vector2) (currentPosition - camera.transform.position)).magnitude;

		//float velocity = distance * acceleration;
		float x = Mathf.SmoothDamp (camera.position.x, targetX, ref yVelocity, damping);
		float y = Mathf.SmoothDamp (camera.position.y, targetY, ref xVelocity, damping);

		camera.position = new Vector3 (x, y, camera.position.z);
	}
}
