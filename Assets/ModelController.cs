using UnityEngine;
using System.Collections;

public class ModelController : MonoBehaviour {

	public Collider[] colliders;
	public MoveJoint[] moveJoints; // All moveJoints in the gameObject

	// Use this for initialization
	void Start () {
		//colliders = gameObject.FindObjectsOfType(Collider) as Collider[];
		moveJoints = GetComponentsInChildren<MoveJoint>();

		// Initialization of variables in every moveJoint
		foreach( MoveJoint moveJoint in moveJoints ) {
			moveJoint.Start();
		}
	}
	
	// Update is called once per frame
	void Update () {
		moveJoints[0].Move(0.0F, 1.0F, 0.0F);
	}
}
