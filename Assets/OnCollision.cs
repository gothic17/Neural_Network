using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class OnCollision : MonoBehaviour {

    private bool touchingFloor;

	void Start() {
        touchingFloor = false;
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.collider.gameObject.name == "Ground") {
            touchingFloor = true;
            /*if (this.name.Equals("Spine") || this.name.Equals("Chest") || this.name.Equals("Neck") || this.name.Equals("Head") ||
			    this.name.Equals("Shoulder.R") || this.name.Equals("Shoulder.L") || this.name.Equals("UpperArm.R") || this.name.Equals("UpperArm.L") || 
				this.name.Equals("LowerArm.R") || this.name.Equals("LowerArm.L")) {*/
            if (this.name.Equals("Spine") || this.name.Equals("Chest") || this.name.Equals("Neck") || this.name.Equals("Head") ||
                this.name.Equals("Shoulder.R") || this.name.Equals("Shoulder.L") || this.name.Equals("UpperArm.r") || this.name.Equals("UpperArm.l")) {
                //print("Aktualizacja sieci - Collider-" + this.name);
                Scene scene = SceneManager.GetActiveScene(); 
				SceneManager.LoadScene(scene.name);
			}
    	// Debug.Log("Collison enter - " + this.gameObject.name);
		}
    }

	/*void OnCollisionStay (Collision collision) {
		if (collision.collider.gameObject.name == "Ground") {
			Debug.Log("Stay occuring... - " + this.gameObject.name);
		}
	}
    */

	void OnCollisionExit (Collision collision) {
		if (collision.collider.gameObject.name == "Ground") {
            touchingFloor = false;
        }
	}

    public bool GetTouchingFloor() {
        return touchingFloor;
    }
}
