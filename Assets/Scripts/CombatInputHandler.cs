using UnityEngine;

[RequireComponent(typeof(CombatController))]
public class CombatInputHandler : MonoBehaviour {
    private CombatController controller;

    void Start() {
        controller = GetComponent<CombatController>();
    }
    
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100, Color.red, 2f);

            if (Physics.Raycast(ray, out hit, 100, 1 << LayerMask.NameToLayer("LevelGeometry"))) {
                Vector2 target = ((Vector2) ray.origin).Floor() + Vector2.one * 0.5f;
                controller.MoveTo(target);
            } else {
                Debug.LogError("Something went wrong");
            }
        }
    }
}