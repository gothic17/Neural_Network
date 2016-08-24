using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ModelController : MonoBehaviour {

	public Collider[] colliders;
	public MoveJoint[] moveJoints; // All moveJoints in the gameObject

	// Use this for initialization
	void Start () {
		//characterJoints = GetComponentsInChildren(typeof(CharacterJoint), true);
		//colliders = gameObject.FindObjectsOfType(Collider) as Collider[];
		moveJoints = GetComponentsInChildren<MoveJoint>();

		// Initialization of variables in every moveJoint
		foreach( MoveJoint moveJoint in moveJoints ) {
			moveJoint.Start();
		}
		
		moveJoints[0].Move(-1.0F, 1.0F, 1.0F);

		// foreach( Collider collider in colliders ) {
		// 	print("---> " + collider);
		// }
	}
	
	// Update is called once per frame
	void Update () {
	}
}
