using UnityEngine;
using System;
using System.Collections.Generic;

public class GlobalScript : MonoBehaviour {

	public static GlobalScript singleton;
	public int dungeonSeed = 1;//DateTime.Now.GetHashCode();
	private List<PlayerVariables> saveState;
	public GameObject dungeon;
	public GameObject battlefield;

	void Awake() {
		if (singleton == null) {
			DontDestroyOnLoad(gameObject);
			singleton = this;
		} else if (singleton != this) {
			Destroy(gameObject);
		}
	}
	
	
	void Update () {
		if (Input.GetKeyDown("space")) {
            GotoCombat(new List<GameObject>{GameObject.Find("PlayerCharacter"), GameObject.Find("NPC")});
		}
	}

	private void GotoCombat(List<GameObject> characters) {
		dungeon.SetActive(false);
		battlefield.SetActive(true);
		var avgPosition = Vector2Utils.Average(
				characters.ConvertAll(new Converter<GameObject, Vector2>(x => x.transform.position)));
		var newCharacters = new List<GameObject>(characters.Count);
		var center = new Vector2(5,5);
		var baseCharacter = Resources.Load("Prefabs/BaseCharacter", typeof(GameObject)) as GameObject;
		var spriteMaterial = Resources.Load("Materials/UnlitSprite", typeof(Material)) as Material;

		foreach (var character in characters) {
			var newCharacter = GameObject.Instantiate(baseCharacter);
			newCharacter.GetComponent<SpriteRenderer>().material = spriteMaterial;
			newCharacter.transform.position = (center + avgPosition.Towards(character.transform.position) * 4f).Round() + Vector2.right * 0.5f + Vector2.up * 0.5f;
			newCharacter.name = "__" + character.name;
			// var controller = character.GetComponent<CharacterController>();
			// controller.acceleration = 1f;
			// controller.friction = 6f;
			if (character.name == "PlayerCharacter") {
				newCharacter.AddComponent(typeof(CombatInputHandler));
			}
			newCharacters.Add(newCharacter);
		}
	}
}
