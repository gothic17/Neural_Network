using UnityEngine;
using System.Collections;

public class OnCollision : MonoBehaviour {

	void Start() {
		//print(this.name);
	}

	void OnCollisionEnter(Collision collision) {
			if (collision.collider.gameObject.name == "Ground") {
    		//Debug.Log("Collison enter - " + this.gameObject.name);
			}
  }

	/*void OnCollisionStay (Collision collision) {
		if (collision.collider.gameObject.name == "Ground") {
			Debug.Log("Stay occuring... - " + this.gameObject.name);
		}
	}

	void OnCollisionExit (Collision collision) {
		if (collision.collider.gameObject.name == "Ground") {
			Debug.Log("Collision exit - " + this.gameObject.name);
		}
	}*/
}
