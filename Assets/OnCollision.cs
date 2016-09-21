using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class OnCollision : MonoBehaviour {

	void Start() {
		//print(this.name);
	}

	void OnCollisionEnter(Collision collision) {
			 if (collision.collider.gameObject.name == "Ground") {
				if (this.name.Equals("Spine") || this.name.Equals("Chest") || this.name.Equals("Neck") || this.name.Equals("Head") ||
					this.name.Equals("Shoulder.R") || this.name.Equals("Shoulder.L") || this.name.Equals("UpperArm.R") || this.name.Equals("UpperArm.L") || 
					this.name.Equals("LowerArm.R") || this.name.Equals("LowerArm.L")) {
					//print("Aktualizacja sieci - Collider-" + this.name);
					Scene scene = SceneManager.GetActiveScene(); 
					//SceneManager.LoadScene(scene.name);
			 	}
    		// Debug.Log("Collison enter - " + this.gameObject.name);
			}

        //if (this.name.Equals("Foot.R")) print(this.name);
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
