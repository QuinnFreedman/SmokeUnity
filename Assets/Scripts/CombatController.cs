using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class CombatController : MonoBehaviour {
    private LinkedList<GridPathNode> path;

    public float maxMoveSpeed = 3f;
    private float moveSpeed = 0;
    public float acceleration = 0.5f;
	private Animator anim;


    void Start() {
		anim = GetComponent<Animator> ();
        anim.SetFloat("LastMoveX", 0);
        anim.SetFloat("LastMoveY", -1);
    }
    
    void Update() {
        if (path != null) {
            var immediateTarget = new Vector2(path.First.Value.X, path.First.Value.Y) + Vector2.one * 0.5f;
            if (((Vector2) transform.position).AproximatelyEqual(immediateTarget, 0.01f)) {
                path.RemoveFirst();
                if (path.Count == 0) {
                    path = null;
                    moveSpeed = 0;
                    anim.SetBool("PlayerMoving", false);
                    return;
                }
                immediateTarget = new Vector2(path.First.Value.X, path.First.Value.Y) + Vector2.one * 0.5f;
            }

            Util.DrawStar(immediateTarget, 0.3f, Color.red, 0.5f);
            foreach (var node in path) {
                Util.DrawStar(new Vector2(node.X, node.Y) + Vector2.one * 0.5f, 0.3f, Color.white);
            }

            var directionToTarget = ((Vector2) transform.position).Towards(immediateTarget);

            anim.SetFloat("MoveX", Util.Crush(directionToTarget.x));
			anim.SetFloat("MoveY", Util.Crush(directionToTarget.y));

            var distanceToTarget = immediateTarget - (Vector2) transform.position;
            moveSpeed = Mathf.Clamp(moveSpeed + acceleration, 0, maxMoveSpeed);
            var moveX = Mathf.Clamp(directionToTarget.x * moveSpeed * Time.deltaTime, -Mathf.Abs(distanceToTarget.x), Mathf.Abs(distanceToTarget.x));
            var moveY = Mathf.Clamp(directionToTarget.y * moveSpeed * Time.deltaTime, -Mathf.Abs(distanceToTarget.y), Mathf.Abs(distanceToTarget.y));
            transform.position += new Vector3(moveX, moveY, 0);
        }
    }

    public void MoveTo(Vector2 target) {
        Util.DrawStar(target, 0.5f, Color.white, 1f);

        var grid = new GridPathNode[10, 10];
        for (int y = 0; y < 10; y++) {
            for (int x = 0; x < 10; x++) {
                grid[x,y] = new GridPathNode(x, y, false);
            }
        }
        var astarGraph = new AStar.SpatialAStar<GridPathNode, System.Object>(grid);
        path = astarGraph.Search(((Vector2) transform.position).Floor(), target, null);
        anim.SetBool("PlayerMoving", true);        
    }
}