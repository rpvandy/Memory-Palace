using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectTextScript : MonoBehaviour {

	public GameObject hand;
	public GameObject[] objectsToSpawn;
	public GameObject text;
	private Text objectText;

	private GameObject activeObject;

	// Use this for initialization
	void Start () {
		activeObject = GameObject.Instantiate (objectsToSpawn [0]);
		objectText = text.GetComponent<Text>();
		objectText.text = activeObject.transform.name;
	}
	
	// Update is called once per frame
	void Update () {
		activeObject.transform.position = hand.transform.position;
		activeObject.transform.rotation = hand.transform.rotation;
	}
}
