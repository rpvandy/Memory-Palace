﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class ObjectTextScript : MonoBehaviour {

	private static bool tutorialScene = true;

	public OVRInput.Controller controller;
	public string[] tutorialSentences;
	public GameObject[] objectSet1;
	public GameObject[] objectSet2;
	public GameObject objectTextField;
	public GameObject tutorialTextField;
	public GameObject blinder;
	public float secondsBetweenSentences;
	public float secondsBetweenObjects;
	public float grabDistance;
	public GameObject[] objectLocations;

	private Text tutorialText;
	private Text objectText;
	private GameObject[] objectsToSpawn;
	private GameObject activeObject;
	private GameObject lastActive = null;
	private float timer;
	private bool givingTutorial;
	private int index = 0;

	// Use this for initialization
	void Start () {
		objectsToSpawn = givingTutorial ? objectSet1 : objectSet2;
		shuffleObjects ();
		tutorialText = tutorialTextField.GetComponent<Text> ();
		objectText = objectTextField.GetComponent<Text>();
		givingTutorial = tutorialScene;
		timer = givingTutorial ? secondsBetweenSentences : secondsBetweenObjects;
		tutorialText.text = givingTutorial ? (tutorialSentences.Length > 0 ? tutorialSentences[0] : "") : "";
		objectText.text = givingTutorial ? "" : Regex.Replace(objectsToSpawn [0].name, @"\(.*\)", "");
		activeObject = givingTutorial ? null : GameObject.Instantiate (objectsToSpawn [0]);
		if (!givingTutorial) {
			activeObject.AddComponent<Rigidbody> ();
			activeObject.GetComponent<Rigidbody> ().useGravity = false;
			activeObject.GetComponent<Rigidbody> ().isKinematic = true;
		}
		for (int i = 0; i < objectLocations.Length; i++) {
			objectLocations [i].SetActive (false);
		}
		blinder.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("TutorialScene")) {
			tutorialScene = true;
			SceneManager.LoadScene (SceneManager.GetActiveScene().name);
		}
		if (Input.GetButtonDown ("TrainingScene")) {
			tutorialScene = false;
			SceneManager.LoadScene (SceneManager.GetActiveScene().name);
		}
		if (givingTutorial) {
			tutorialStep ();
		} else {
			objectStep ();
		}
	}

	void tutorialStep() {
		if (timer <= 0) {
			if (index >= tutorialSentences.Length) {
				givingTutorial = false;
				index = 0;
				activeObject = GameObject.Instantiate (objectsToSpawn [0]);
				activeObject.AddComponent<Rigidbody> ();
				activeObject.GetComponent<Rigidbody> ().useGravity = false;
				activeObject.GetComponent<Rigidbody> ().isKinematic = true;
				objectLocations [0].SetActive (true);
				timer = secondsBetweenObjects;
				tutorialText.text = "";
				objectText.text = Regex.Replace(activeObject.name,  @"\(.*\)", "");
			} else {
				index++;
				tutorialText.text = tutorialSentences [index];
				timer = secondsBetweenSentences;
			}
		} else {
			timer -= Time.deltaTime;
		}
	}

	void objectStep() {
		if (activeObject != null) {
			activeObject.GetComponent<Rigidbody> ().MovePosition (OVRInput.GetLocalControllerPosition (controller));
			activeObject.GetComponent<Rigidbody> ().MoveRotation (OVRInput.GetLocalControllerRotation (controller));
			if (Input.GetButtonDown ("Fire1") || Input.GetAxis("RHandTrigger") == 1) {
				if (activeObject.CompareTag (index.ToString ())) {
					lastActive = activeObject;
					activeObject = null;
				}
			}
		} else {
			if ((Input.GetButtonDown ("Fire1") || Input.GetAxis("RHandTrigger") == 1) && Vector3.Distance (lastActive.transform.position, OVRInput.GetLocalControllerPosition (controller)) < grabDistance) {
				activeObject = lastActive;
				lastActive = null;
				activeObject.transform.position = OVRInput.GetLocalControllerPosition (controller);
				activeObject.transform.rotation = OVRInput.GetLocalControllerRotation (controller);
			}
		}
		if (timer <= 0) {
			if (index < objectsToSpawn.Length && index < objectLocations.Length) {
				if (!activeObject.CompareTag (index.ToString ())) {
					activeObject.transform.position = objectLocations [index].transform.position;
				}
				objectLocations [index].SetActive (false);
				index++;
				objectLocations [index].SetActive (true);
				objectText.text = Regex.Replace (objectsToSpawn [index].name, @"\(.*\)", "");
				activeObject = GameObject.Instantiate (objectsToSpawn [index]);
				activeObject.AddComponent<Rigidbody> ();
				activeObject.GetComponent<Rigidbody> ().useGravity = false;
				activeObject.GetComponent<Rigidbody> ().isKinematic = true;
				timer = secondsBetweenObjects;
			} else {
				blinder.SetActive (true);
			}
		} else {
			timer -= Time.deltaTime;
		}
	}

	void shuffleObjects() {
		int objects = objectsToSpawn.Length;
		int swapIndex;
		while (objects > 0) {
			swapIndex = Mathf.FloorToInt (Random.value * objects);
			objects--;
			GameObject temp = objectsToSpawn [objects];
			objectsToSpawn [objects] = objectsToSpawn [swapIndex];
			objectsToSpawn [swapIndex] = temp;
		}
	}
}
