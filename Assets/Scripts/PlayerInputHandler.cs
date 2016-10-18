using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerInputHandler : MonoBehaviour {

    private CharacterController controller;

    void Start() {
        controller = GetComponent<CharacterController>();
    }

    void FixedUpdate() {
        controller.Move(new Vector2(Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical")));
    }
}