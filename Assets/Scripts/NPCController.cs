using UnityEngine;
using System.Collections.Generic;
using System;
using AStar;

[RequireComponent(typeof(CharacterController))]
public class NPCController : MonoBehaviour {

	private Transform target;
	private Mode mode = Mode.IDLE;
	private CharacterController controller;
	private Maybe<Vector2> idleTarget;
	private int timeTillIdleMove;
	private float maxColiderDistance;
	public float followStartDistance = 4f;
	public float followStopDistance = 7f;
	public float maxFollowDistance = 1f;
	public float minFollowDistance = 0.8f;
	public bool requireLosToStartFollowing = true;
	public bool roamWhenIdle = true;
	public float roamDistance = 3f;
	public int framesWaitWhileIdle = 120;
	public static SpatialAStar<GridPathNode, System.Object> aStar = null;

	enum Mode {
		FOLLOW, IDLE
	}

	void Start () {
		target = GameObject.Find("player").transform;
		controller = GetComponent<CharacterController>();
		if (aStar == null) {
			//TODO 
		}
		idleTarget = Maybe<Vector2>.NONE();
		var colider = GetComponent<CircleCollider2D>();
		maxColiderDistance = Mathf.Sqrt(2 * colider.radius * colider.radius);
		timeTillIdleMove = 0;
	}
	
	Point selfPosition;
	Point targetPosition;
	LinkedList<GridPathNode> path = null;
	void FixedUpdate () {
		Vector2 moveDirection = Vector2.zero;

		//distance to target
		float distance = Vector2.Distance(target.transform.position, transform.position);
		//direction to target
		Vector3 directionToTarget = Towards(target.transform.position);//(target.transform.position - transform.position).normalized;
		//racast toward target
		RaycastHit2D hitinfo = Physics2D.Raycast(transform.position, directionToTarget, distance, 1 << LayerMask.NameToLayer("LevelGeometry"));
		if (mode == Mode.FOLLOW) {
			controller.walking = false;
			if (distance > followStopDistance && followStopDistance > 0) {
				mode = Mode.IDLE;
				//goto case Mode.SEARCH;
			}

			if (distance <= maxFollowDistance) {
				if (distance < minFollowDistance) {
					//move away from target
					moveDirection = -directionToTarget;
				}
			} else {
				//move toward target
				if (hitinfo) {
					//no line of sight to target
					Debug.DrawRay(transform.position, directionToTarget, Color.red, 0.5f);
					var _targetPosition = Point.FromVector2(target.transform.position);
					var _selfPosition = Point.FromVector2(transform.position);
					if (!_targetPosition.Equals(targetPosition) || !_selfPosition.Equals(selfPosition) || path == null) {
						targetPosition = _targetPosition;
						selfPosition = _selfPosition;
						try {
							path = aStar.Search(transform.position, target.transform.position, null);
							//path = AStar.GetPath(_selfPosition, _targetPosition, 200);
						} catch (Exception e) {
							Debug.LogError("Error in AStar: "+e);
						}
					}
					if(path != null) {
						var enumerator = path.GetEnumerator();
						GridPathNode previousPoint = null;
						while (enumerator.MoveNext())
						{
							var node = enumerator.Current;
							Debug.DrawLine(new Vector3(node.X + 0.75f, node.Y + 0.25f, 0), new Vector3(node.X + 0.25f, node.Y + 0.75f, 0));
							Debug.DrawLine(new Vector3(node.X + 0.25f, node.Y + 0.25f, 0), new Vector3(node.X + 0.75f, node.Y + 0.75f, 0));
							if(previousPoint != null) {
								Debug.DrawLine(new Vector3(previousPoint.X + 0.5f, previousPoint.Y + 0.5f, 0), new Vector3(node.X + 0.5f, node.Y + 0.5f, 0));
							}
						}
						//move the character towrads the second node in the path
						//(the first node is the square closest to where the character is standing)
						var targetNode = path.First.Next.Value;
						var immediateTarget = new Vector3(targetNode.X + 0.5f, targetNode.Y + 0.5f);
						moveDirection = (immediateTarget - transform.position).normalized;
					}
				} else {
					//open line of sight to target
					Debug.DrawRay(transform.position, directionToTarget, Color.green, 0.5f);
					moveDirection = directionToTarget;
				}
			}
		} else if (mode == Mode.IDLE) {
			controller.walking = true;
			if(distance <= followStartDistance && !hitinfo || !requireLosToStartFollowing) {
				mode = Mode.FOLLOW;
			}

			if (roamWhenIdle) {
				if (idleTarget is Maybe<Vector2>.None) {
					if(timeTillIdleMove == 0) {
						timeTillIdleMove = UnityEngine.Random.Range(framesWaitWhileIdle/2, framesWaitWhileIdle);
						Vector2 directionToIdleTarget;
						float vectorLength;
						do {
							directionToIdleTarget = Vector2Extension.Random();
							vectorLength = UnityEngine.Random.Range(roamDistance/2, roamDistance);
							idleTarget = Maybe<Vector2>.SOME((Vector2) transform.position + directionToIdleTarget * vectorLength);
							// idleTarget = Maybe<Vector2>.SOME(new Vector2(
							// 		transform.position.x + UnityEngine.Random.Range(-roamDistance, roamDistance),
							// 		transform.position.y + UnityEngine.Random.Range(-roamDistance, roamDistance)));
							// directionToIdleTarget = Towards(((Maybe<Vector2>.Some) idleTarget).Value);
						} while (Physics2D.Raycast(transform.position, directionToIdleTarget, vectorLength,
								1 << LayerMask.NameToLayer("LevelGeometry")).collider != null);
						Debug.DrawLine(transform.position, ((Maybe<Vector2>.Some) idleTarget).Value, Color.white, 1.5f);
						moveDirection = directionToIdleTarget;
					} else {
						timeTillIdleMove--;
					}
				} else {
					var idleTargetVec = ((Maybe<Vector2>.Some) idleTarget).Value;
					moveDirection = Towards(idleTargetVec);
					if (AreClose(transform.position, idleTargetVec, maxColiderDistance + 0.05f)) {
						idleTarget = Maybe<Vector2>.NONE();
					}
				}
			}
		}
		controller.Move(moveDirection);
	}

	void OnDrawGizmos() {
		switch (mode) {
			case Mode.FOLLOW:
				Gizmos.color = Color.red;
			break;
			case Mode.IDLE:
				Gizmos.color = Color.green;
			break;
		}
		Gizmos.DrawSphere(GetComponent<Transform>().position + Vector3.up * 0.7f, 0.2f);
	}

	Vector2 Towards(Vector2 toward) {
		return (toward - (Vector2) transform.position).normalized;
	}

	bool AreClose(Vector2 a, Vector2 b, float maxDistance) {
		return Mathf.Abs(a.x - b.x) <= maxDistance && Mathf.Abs(a.y - b.y) <= maxDistance;
	}
}