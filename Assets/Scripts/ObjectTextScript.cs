using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class ObjectTextScript : MonoBehaviour {

	private static bool tutorialScene = true;

	public Transform hand;
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
	public float placementRadius = 0.5f;

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
			
			activeObject.transform.position = hand.position;
			activeObject.transform.rotation = hand.rotation;
			if (Input.GetButtonDown ("Fire1") || Input.GetAxis("RHandTrigger") == 1 || OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5f) {
				print (Vector3.Distance(new Vector3(activeObject.transform.position.x, activeObject.transform.position.y, 0f),
					new Vector3(objectLocations[index].transform.position.x, objectLocations[index].transform.position.y, 0f)));
				if (Vector3.Distance(new Vector3(activeObject.transform.position.x, activeObject.transform.position.y, 0f),
					new Vector3(objectLocations[index].transform.position.x, objectLocations[index].transform.position.y, 0f)) < placementRadius) {
					print ("Here");
					lastActive = activeObject;
					activeObject = null;
				}
			}
		} else {
			if ((Input.GetButtonDown ("Fire1") || Input.GetAxis("RHandTrigger") == 1|| OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > 0.5f) && Vector3.Distance (lastActive.transform.position, hand.position) < grabDistance) {
				print ("There");
				activeObject = lastActive;
				lastActive = null;
				activeObject.transform.position = hand.position;
				activeObject.transform.rotation = hand.rotation;
			}
		}
		if (timer <= 0) {
			if (index < objectsToSpawn.Length && index < objectLocations.Length) {
				if (activeObject != null && Vector3.Distance(new Vector3(activeObject.transform.position.x, activeObject.transform.position.y, 0f),
					new Vector3(objectLocations[index].transform.position.x, objectLocations[index].transform.position.y, 0f)) > placementRadius) {
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
