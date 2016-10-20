using UnityEngine;
using System.Collections.Generic;
using System;
using AStar;

[RequireComponent(typeof(CharacterController))]
public class NPCController : MonoBehaviour {

	private Transform target;
	private Mode mode = Mode.FOLLOW;
	private CharacterController controller;
	public float maxFollowDistance = 1f;
	public float minFollowDistance = 0.8f;

	public static SpatialAStar<GridPathNode, System.Object> aStar = null;

	enum Mode {
		FOLLOW, SEARCH
	}

	void Start () {
		target = GameObject.Find("player").transform;
		controller = GetComponent<CharacterController>();
		if (aStar == null) {
			//TODO 
		}
	}
	
	Point selfPosition;
	Point targetPosition;
	LinkedList<GridPathNode> path = null;
	void FixedUpdate () {
		Vector2 moveDirection = Vector2.zero;
		switch (mode) {
			case Mode.FOLLOW:
				//direction to target
				Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
				//distance to target
				float distance = Vector2.Distance(target.transform.position, transform.position);
				if (distance <= maxFollowDistance) {
					if (distance < minFollowDistance) {
						//move away from target
						moveDirection = -directionToTarget;
					}
				} else {
					//move toward target
					RaycastHit2D hitinfo = Physics2D.Raycast(transform.position, directionToTarget, distance, 1 << LayerMask.NameToLayer("LevelGeometry"));
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
			break;
		}
		controller.Move(moveDirection);
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
