using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementTrigger : MonoBehaviour {
	public int triggerOrder;

	void OnTriggerEnter(Collider other) {
		other.gameObject.tag = triggerOrder.ToString();
	}

	void OnTriggerExit(Collider other) {
		other.gameObject.tag = null;
	}
}
