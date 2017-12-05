using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ObjectTextScript : MonoBehaviour {

	private static bool tutorialScene = true;

	public GameObject hand;
	public string[] tutorialSentences;
	public GameObject[] objectsToSpawn;
	public GameObject objectTextField;
	public GameObject tutorialTextField;
	public float secondsBetweenSentences;
	public float secondsBetweenObjects;
	public float grabDistance;
	public GameObject[] objectLocations;

	private Text tutorialText;
	private Text objectText;

	private GameObject activeObject;
	private GameObject lastActive = null;
	private float timer;
	private bool givingTutorial;
	private int index = 0;

	// Use this for initialization
	void Start () {
		tutorialText = tutorialTextField.GetComponent<Text> ();
		objectText = objectTextField.GetComponent<Text>();
		givingTutorial = tutorialScene;
		timer = givingTutorial ? secondsBetweenSentences : secondsBetweenObjects;
		tutorialText.text = givingTutorial ? tutorialSentences [0] : "";
		objectText.text = givingTutorial ? "" : objectsToSpawn [0].name;
		activeObject = givingTutorial ? null : GameObject.Instantiate (objectsToSpawn [0]);
		if (!givingTutorial) {
			activeObject.AddComponent<Rigidbody> ();
			activeObject.GetComponent<Rigidbody> ().useGravity = false;
			activeObject.GetComponent<Rigidbody> ().isKinematic = true;
		}
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
			//print ("Object step");
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
				timer = secondsBetweenObjects;
				tutorialText.text = "";
				objectText.text = activeObject.name;
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
			activeObject.GetComponent<Rigidbody> ().MovePosition (hand.transform.position);
			activeObject.GetComponent<Rigidbody> ().MoveRotation (hand.transform.rotation);
			if (Input.GetButtonDown ("Fire1")) {
				if (activeObject.CompareTag (index.ToString ())) {
					lastActive = activeObject;
					activeObject = null;
				}
			}
		} else {
			if (Input.GetButtonDown("Fire1") && Vector3.Distance (lastActive.transform.position, hand.transform.position) < grabDistance) {
				activeObject = lastActive;
				lastActive = null;
				activeObject.transform.position = hand.transform.position;
				activeObject.transform.rotation = hand.transform.rotation;
			}
		}
		if (timer <= 0) {
			if (index < objectsToSpawn.Length) {
				if (!activeObject.CompareTag (index.ToString ())) {
					activeObject.transform.position = objectLocations [index].transform.position;
				}
				index++;
				objectText.text = objectsToSpawn[index].name;
				activeObject = GameObject.Instantiate (objectsToSpawn [index]);
				activeObject.AddComponent<Rigidbody> ();
				activeObject.GetComponent<Rigidbody> ().useGravity = false;
				activeObject.GetComponent<Rigidbody> ().isKinematic = true;
				timer = secondsBetweenObjects;
			}
		} else {
			timer -= Time.deltaTime;
		}
	}
}
