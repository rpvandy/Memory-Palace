using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript2 : MonoBehaviour {
	bool isCreated;
	bool ready;
	public GameObject sphere;
	public GameObject walls;
	public GameObject cube;
	int count;
	GameObject sphereClone;
	// Use this for initialization
	void Start () {
		bool isCreated=false;
		ready = false;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 playerPosition = Camera.main.transform.position;
		Vector3 playerDirection = Camera.main.transform.forward;
		Vector3 position = playerPosition + playerDirection*2;
		if (!ready) {
		}else {
			sphereClone.transform.position = position;
		}
		if (Input.GetMouseButton (0)) {
			if (!isCreated) {
				sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				sphereClone = Instantiate (sphere, position, Quaternion.identity) as GameObject;
				isCreated = true;
				ready = true;
			}
		}
	}
}
